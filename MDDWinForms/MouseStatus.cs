using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MDDWinForms
{

    public class MouseStatus
    {
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);

        private Point _lastPosition;
        private long _lastMoveTick;
        private readonly int _minShowDistance;
        private readonly int _minHideDistance;
        private readonly int _minShowIntervalMs;
        private readonly int _minHideIntervalMs;
        private static bool _Visible;
        private int _accumulatedDistanceSquared;
        public MouseStatus(int minShowDistance = 5, int minHideDistance = 3, int minShowIntervalMs = 100, int minHideIntervalMs = 1000)
        {
            _minShowDistance = minShowDistance;
            _minHideDistance = minHideDistance;
            _minShowIntervalMs = minShowIntervalMs;
            _minHideIntervalMs = minHideIntervalMs;
            _lastPosition = Cursor.Position;
            _lastMoveTick = Environment.TickCount;
            _Visible = true;
            _accumulatedDistanceSquared = 0;
        }

        /// <summary>
        /// Call this from OnMouseMove (must be on the UI thread)
        /// </summary>
        public void MaybeShowCursor(Control context)
        {
            //need to do this no matter what - if the cursor is visible, we're tracking for MaybeHideCursor,
            //and if it's hidden, we're tracking for MaybeShowCursor
            Point currentPosition = Cursor.Position;
            if (_lastPosition.X != 0 && _lastPosition.Y != 0)
            {
                int dx = currentPosition.X - _lastPosition.X;
                int dy = currentPosition.Y - _lastPosition.Y;
                int distanceSquared = dx * dx + dy * dy;
                _accumulatedDistanceSquared += distanceSquared;
            }
            else
            {
                _accumulatedDistanceSquared = 0;
                _lastMoveTick = Environment.TickCount;
            }
            _lastPosition = currentPosition;            
            
            if (_Visible)
            {
                //can't reset lastMoveTick here because we need to accumulate distance for the MaybeHideCursor threshold
                return;
            }

            Rectangle clientRect = context.RectangleToScreen(context.ClientRectangle);
            //if (Debugger.IsAttached) Console.WriteLine($"Rect: {clientRect} Pos: {currentPosition}");
            if (!clientRect.Contains(currentPosition))
            {
                //if the cursor is outside of the context window, show immediately
                if (Debugger.IsAttached) Console.WriteLine("Showing (outside)...");
                Cursor.Show();
                _Visible = true;
                _lastMoveTick = Environment.TickCount;
                _accumulatedDistanceSquared = 0;
                return;
            }

            long now = Environment.TickCount;
            if ((now - _lastMoveTick) < _minShowIntervalMs)
                return;

            //if the cursor is not visible but the time threshold has been met, we reset the lastMoveTick regardless of distance
            _lastMoveTick = now;

            if (_accumulatedDistanceSquared >= _minShowDistance * _minShowDistance)
            {
                if (Debugger.IsAttached) Console.WriteLine("Showing...");
                Cursor.Show();
                _Visible = true;
            }
            //if the cursor was not visible and the time threshold was met, we reset the distance threshold regardless of whether it was shown
            //this is intentional since the whole point of this is to not show the cursor if the user is not really moving it
            _accumulatedDistanceSquared = 0;
       }

        /// <summary>
        /// Call this periodically from a WinForms timer only (must be on the UI thread).
        /// </summary>
        public void MaybeHideCursor(Control context)
        {
            if (!_Visible)
                return;

            long now = Environment.TickCount;
            if ((now - _lastMoveTick) < _minHideIntervalMs)
                return;

            // Only hide if the cursor is over the control's client area
            Rectangle clientRect = context.RectangleToScreen(context.ClientRectangle);
            Point cursorPos = Cursor.Position;

            if (!clientRect.Contains(cursorPos))
            {
                _accumulatedDistanceSquared = 0; // Still reset distance
                _lastPosition = new Point(0, 0);
                return;
            }

            // Check if this control is the topmost window under the cursor
            IntPtr hwndUnderCursor = WindowFromPoint(cursorPos);
            if (hwndUnderCursor != context.Handle)
            {
                _accumulatedDistanceSquared = 0;
                _lastPosition = new Point(0, 0);
                return;
            }

            //if the time threshold has been met, it needs to be reset to restart the cycle
            _lastMoveTick = now;

            //Debug.Print($"Hide distance: {_accumulatedDistanceSquared}");

            if (_accumulatedDistanceSquared < _minHideDistance * _minHideDistance)
            {
                if (Debugger.IsAttached) Console.WriteLine($"Hiding...{context.Tag}");
                Cursor.Hide();
                _Visible = false;
            }
            //if the time threshold has been met, the distance threshold needs to be reset regardless
            _accumulatedDistanceSquared = 0;
        }
    }
}