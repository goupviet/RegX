using System;
using System.Windows.Controls;
using System.Windows;

namespace RegX
{
    class MatchLabel : Label
    {
        public MatchLabel(int Index, int Length, string MatchedContent)
        {
            this.Index = Index;
            this.Length = Length;
            this.MatchedContent = MatchedContent;
            Height = 25;
            Content = "Index: " + Index + " Length: " + Length + " Content: " + MatchedContent;
        }

        public int Index { get; set; }

        public int Length { get; set; }

        public string MatchedContent { get; set; }
    }
}
