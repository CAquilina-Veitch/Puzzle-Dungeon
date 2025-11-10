using UnityEngine.Splines;

namespace Scripts.Editor
{
    /// <summary>
    /// Simple clipboard for copying and pasting spline data
    /// </summary>
    public static class SplineClipboard
    {
        private static Spline copiedSpline;

        public static bool HasSpline => copiedSpline != null;

        public static void CopySpline(Spline source)
        {
            if (source == null)
            {
                copiedSpline = null;
                return;
            }

            // Create a new spline and copy all data
            copiedSpline = new Spline();
            foreach (var knot in source)
            {
                copiedSpline.Add(knot);
            }
            copiedSpline.Closed = source.Closed;
        }

        public static Spline GetCopiedSpline()
        {
            if (copiedSpline == null)
                return null;

            // Return a new copy so the clipboard data isn't modified
            var newSpline = new Spline();
            foreach (var knot in copiedSpline)
            {
                newSpline.Add(knot);
            }
            newSpline.Closed = copiedSpline.Closed;

            return newSpline;
        }

        public static void PasteSplineInto(Spline target)
        {
            if (target == null || copiedSpline == null)
                return;

            target.Clear();
            foreach (var knot in copiedSpline)
            {
                target.Add(knot);
            }
            target.Closed = copiedSpline.Closed;
        }
    }
}
