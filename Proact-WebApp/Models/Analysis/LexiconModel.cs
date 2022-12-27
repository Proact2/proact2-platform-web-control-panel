using System;
using System.Collections.Generic;
using System.Drawing;

namespace Proact_WebApp.Models {

    public enum LexiconState {
        DRAW,
        PUBLISHED
    }

    public class LexiconModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public LexiconState State { get; set; }
        public DateTime Created { get; set; }
        public List<LexiconCategoryModel> Categories { get; set; }
    }

    public class LexiconCategoryModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool MultipleSelection { get; set; }
        public int Order { get; set; }
        public List<LexiconLabelModel> Labels { get; set; }
    }

    public class LexiconLabelModel {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string GroupName { get; set; }


        public string BgGroupColor {
            get {
                if ( GroupName == null ) {
                    return "#FFFFFF";
                }
                var color = GenerateGroupColor();
                return ColorToHex( color );
            }
        }

        public string TextGroupColor {
            get {
                if ( GroupName == null ) {
                    return "#FFFFFF";
                }
                var color = GenerateGroupColor();
                var textColor = ContrastColor( color );
                return ColorToHex( textColor );
            }
        }

        private Color GenerateGroupColor() {
            return Color.FromArgb( GroupName.GetHashCode() );
        }

        private string ColorToHex(Color color) {
           return string.Format( "#{0:X2}{1:X2}{2:X2}",
                  color.R, color.G, color.B );
        }

        private Color ContrastColor( Color color ) {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color...      
            double luminance = ( 0.299 * color.R + 0.587 * color.G + 0.114 * color.B ) / 255;

            if ( luminance > 0.5 )
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb( d, d, d );
        }
    }
}
