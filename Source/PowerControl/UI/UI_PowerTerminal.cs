using System;
using PowerControl.Buildings;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using PowerControl.Utils;

namespace PowerControl.UI
{
    public class UI_PowerTerminal : Window
    {
        private static List<UI_PowerTerminal> openTerminals = new List<UI_PowerTerminal>();

        [Range(0f, 1f)]
        public float VerticalSplit = 0.6f;

        public float SectionAWidth = 100;
        public float SectionBWidth = 100;

        public float MinVerticalSize = 100f;
        public float MinSectionWidth = 50f;

        private int currentlyDragging = -1;
        private int draggableIndex;
        private float sectionBStart;
        private float sectionAStart;

        public override Vector2 InitialSize => new Vector2(1000, 900);

        public static UI_PowerTerminal Open(PowerTerminal terminal)
        {
            //if (terminal == null)
            //{
            //    ModCore.Warn("Called UI_PowerTerminal.Open() with null terminal.");
            //    return null;
            //}

            var current = GetOpenUI(terminal);
            if (current != null)
            {
                // Focus existing.
                // Does this work?
                Find.WindowStack.Notify_ManuallySetFocus(current);
                return current;
            }
            else
            {
                // Open new.
                var created = new UI_PowerTerminal();
                created.Terminal = terminal;

                openTerminals.Add(created);
                Find.WindowStack.Add(created);

                return created;
            }
        }

        public static UI_PowerTerminal GetOpenUI(PowerTerminal terminal)
        {
            if (terminal == null || !terminal.Spawned)
                return null;

            foreach (var ui in openTerminals)
            {
                if (ui.Terminal == terminal)
                    return ui;
            }

            return null;
        }

        public PowerTerminal Terminal;

        private UI_PowerTerminal()
        {
            doCloseButton = false;
            doCloseX = true;
            onlyOneOfTypeAllowed = false;
            draggable = false;
            preventCameraMotion = false;
            resizeable = true;
            absorbInputAroundWindow = false;
            focusWhenOpened = true;
            closeOnClickedOutside = false;
            closeOnAccept = true;
            closeOnCancel = true;
        }

        public override void PreClose()
        {
            openTerminals.Remove(this);
            base.PreClose();
        }

        public override void PostOpen()
        {
            base.PostOpen();

            optionalTitle = "Power Control Terminal";
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawUI(inRect);
        }

        void DrawUI(Rect rect)
        {
            const float PADDING = 5f;
            draggableIndex = 0;
            float topHeight = rect.height * VerticalSplit;
            Rect topArea = new Rect(rect.x, rect.y, rect.width, topHeight);
            float bottomHeight = rect.height - topHeight;
            Rect bottomArea = new Rect(rect.x, rect.y + topHeight, rect.width, bottomHeight);

            // Net power.
            float netWatts = 123.5f;
            string netPowerTxt = PowerConvert.GetPrettyPower(netWatts);
            Text.Font = GameFont.Medium;
            var txtSize = Text.CalcSize(netPowerTxt);
            Widgets.Label(new Rect(rect.xMax - txtSize.x - PADDING, rect.y + PADDING, txtSize.x, txtSize.y), netPowerTxt);

            // Vertical slide.
            float vertSize = Mathf.Clamp(Draggable(new Vector2(rect.x, rect.y + topHeight), rect.width, 0) - rect.y, MinVerticalSize, rect.height - MinVerticalSize);
            VerticalSplit = vertSize / rect.height;

            // First section slide.
            float maxSecAWidth = rect.width - SectionCWidth() - MinSectionWidth;
            SectionAWidth = Mathf.Clamp(Draggable(new Vector2(rect.x + SectionAWidth, rect.y + topHeight), 0f, bottomHeight, () =>
            {
                sectionBStart = SectionBWidth;
                sectionAStart = SectionAWidth;
            }) - rect.x, MinSectionWidth, maxSecAWidth);
            if (IsDragging())
            {
                float deltaA = SectionAWidth - sectionAStart;
                float adjustedB = sectionBStart - deltaA;
                SectionBWidth = adjustedB;
            }

            // Second section slide.
            float cWidth = rect.xMax - Draggable(new Vector2(rect.x + SectionAWidth + SectionBWidth, rect.y + topHeight), 0f, bottomHeight);
            float bWidth = rect.width - cWidth - SectionAWidth;
            SectionBWidth = Mathf.Clamp(bWidth, MinSectionWidth, rect.width - SectionAWidth - MinSectionWidth);

            Rect sectionA = new Rect(rect.x, rect.y + topHeight, SectionAWidth, bottomHeight);
            Rect sectionB = new Rect(rect.x + SectionAWidth, rect.y + topHeight, SectionBWidth, bottomHeight);
            Rect sectionC = new Rect(rect.x + SectionAWidth + SectionBWidth, rect.y + topHeight, SectionCWidth(), bottomHeight);

            List<SimpleCurveDrawInfo> curves = new List<SimpleCurveDrawInfo>();
            var curve = new SimpleCurve();
            var curve2 = new SimpleCurve();

            for (int i = 0; i < 100; i++)
            {
                float x = i;
                float y = x * x;
                float y2 = x * 80;

                curve.Add(new CurvePoint(x, y));
                curve2.Add(new CurvePoint(x, y2));
            }

            var drawInfo = new SimpleCurveDrawInfo();
            drawInfo.curve = curve;
            drawInfo.color = Color.cyan;
            drawInfo.label = "Power Production";
            curves.Add(drawInfo);

            var drawInfo2 = new SimpleCurveDrawInfo();
            drawInfo2.curve = curve2;
            drawInfo2.color = Color.red;
            drawInfo2.label = "Power Consumption";
            curves.Add(drawInfo2);

            var style = new SimpleCurveDrawerStyle();
            //style.UseFixedScale = true;
            style.DrawBackground = true;
            style.DrawLegend = true;
            style.DrawMeasures = true;
            style.DrawPoints = false;
            style.DrawCurveMousePoint = true;
            style.UseAntiAliasedLines = true;

            SimpleCurveDrawer.DrawCurves(topArea, curves, style);

            //GUI.Box(rect, "");
            GUI.Box(topArea, "");
            GUI.Box(sectionA, "");
            GUI.Box(sectionB, "");
            GUI.Box(sectionC, "");

            float SectionCWidth()
            {
                return rect.width - SectionAWidth - SectionBWidth;
            }
        }

        bool IsDragging()
        {
            return currentlyDragging == draggableIndex;
        }

        float Draggable(Vector2 start, float width, float height, Action startDrag = null)
        {
            if (width != 0 && height != 0)
                throw new System.Exception("Width and height cannot both be non-zero.");

            if (width == 0 && height == 0)
                throw new System.Exception("Width and height cannot both be zero.");

            int thisIndex = ++draggableIndex;

            var e = Event.current;
            bool isVertical = height != 0;

            const float THICC = 20f;
            const float HALF_THICC = THICC * 0.5f;

            Rect drawArea;
            if (isVertical)
                drawArea = new Rect(start.x - HALF_THICC, start.y, THICC, height);
            else
                drawArea = new Rect(start.x, start.y - HALF_THICC, width, THICC);

            bool isDraggingSelf = currentlyDragging == thisIndex;
            bool isDraggingAny = currentlyDragging > 0;

            // Update when dragging.
            if (isDraggingSelf)
            {
                // Detect stop drag.
                if (e.button == 0 && e.type == EventType.MouseUp)
                {
                    currentlyDragging = -1;
                }
                else
                {
                    if (isVertical)
                        start.x = e.mousePosition.x;
                    else
                        start.y = e.mousePosition.y;
                }

                var oldColor = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 0.8f);
                GUI.Box(drawArea, "");
                GUI.color = oldColor;
            }

            bool mouseInArea = drawArea.Contains(e.mousePosition);

            // Draw drag hint.
            if (!isDraggingAny && mouseInArea)
            {
                var oldColor = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 0.35f);
                GUI.Box(drawArea, "");
                GUI.color = oldColor;
            }

            // Detect start drag.
            if (!isDraggingAny && e.button == 0 && e.type == EventType.MouseDown && mouseInArea)
            {
                currentlyDragging = thisIndex;
                startDrag?.Invoke();
            }

            return isVertical ? start.x : start.y;
        }
    }
}
