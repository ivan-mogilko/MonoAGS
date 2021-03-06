﻿using System;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public static class GameViewColors
    {
        public static Color Panel = Color.FromRgba(53, 64, 81, 250);
        public static Color Border = Color.FromRgba(44, 51, 61, 255);
        public static Color Text = Colors.WhiteSmoke;
        public static Color HoveredText = Colors.Yellow;
        public static Color ReadonlyText = Colors.LightGray;
        public static Color Button = Border;
        public static Color HoveredButton = Colors.Gray;
        public static Color PushedButton = Colors.DarkGray;
        public static Color SubPanel = Border;
        public static Color Textbox = Border;
        public static Color Menu = Border;
        public static Color HoveredMenuItem = Colors.DarkBlue;
        public static Color TextboxColor = Color.FromHexa(0x2d323a).WithAlpha(255);
        public static Color TextboxHoverColor = Color.FromHexa(0x505660).WithAlpha(255);
        public static IBorderStyle TextboxBorder = AGSGame.Game.Factory.Graphics.Borders.SolidColor(TextboxColor, 3f, true);
        public static IBorderStyle TextboxHoverBorder = AGSGame.Game.Factory.Graphics.Borders.SolidColor(TextboxHoverColor, 3f, true);

        public static IBrush TextBrush = AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(Text);
        public static IBrush HoveredTextBrush = AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(HoveredText);
        public static IBrush ReadonlyTextBrush = AGSGame.Game.Factory.Graphics.Brushes.LoadSolidBrush(ReadonlyText);

        public static ITextConfig ButtonTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: TextBrush);
        public static ITextConfig TextboxTextConfig = new AGSTextConfig(autoFit: AutoFit.TextShouldCrop, brush: TextBrush);
        public static ITextConfig ReadonlyTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: ReadonlyTextBrush,
             font: AGSGame.Device.FontLoader.LoadFont(AGSGame.Game.Settings.Defaults.TextFont.FontFamily, 12f));
        public static ITextConfig ButtonHoverTextConfig = new AGSTextConfig(autoFit: AutoFit.LabelShouldFitText, brush: HoveredTextBrush);
        public static ITextConfig TextboxHoverTextConfig = new AGSTextConfig(autoFit: AutoFit.TextShouldCrop, brush: HoveredTextBrush);

        public static void AddHoverEffect(ITextBox textbox)
        {
            textbox.TextBackgroundVisible = true;
            textbox.Tint = TextboxColor;
            textbox.Border = TextboxBorder;
            var uiEvents = textbox.GetComponent<IUIEvents>();
            uiEvents.MouseEnter.Subscribe(_ =>
            {
                textbox.TextConfig = TextboxHoverTextConfig;
                textbox.Tint = TextboxHoverColor;
                textbox.Border = TextboxHoverBorder;
            });
            uiEvents.MouseLeave.Subscribe(_ => 
            {
                textbox.TextConfig = TextboxTextConfig;
                textbox.Tint = TextboxColor;
                textbox.Border = TextboxBorder;
            });
        }
    }
}
