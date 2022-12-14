#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Sanford.Multimedia.Midi.UI;

public partial class PianoControl
{
    private sealed class PianoKey : Control
    {
        private readonly SolidBrush offBrush = new(Color.White);

        private readonly SolidBrush onBrush = new(Color.SkyBlue);
        private readonly PianoControl owner;
        private int noteID = 60;

        public PianoKey(PianoControl owner)
        {
            this.owner = owner;
            TabStop = false;
        }

        public Color NoteOnColor
        {
            get => onBrush.Color;
            set
            {
                onBrush.Color = value;

                if (IsPianoKeyPressed) Invalidate();
            }
        }

        public Color NoteOffColor
        {
            get => offBrush.Color;
            set
            {
                offBrush.Color = value;

                if (!IsPianoKeyPressed) Invalidate();
            }
        }

        public int NoteID
        {
            get { return noteID; }
            set
            {
                #region Require

                if (value is >= 0 and <= ShortMessage.DataMaxValue)
                    noteID = value;
                else
                    throw new ArgumentOutOfRangeException("NoteID", noteID,
                        "Note ID out of range.");

                #endregion
            }
        }

        public bool IsPianoKeyPressed { get; private set; }

        public void PressPianoKey()
        {
            #region Guard

            if (IsPianoKeyPressed) return;

            #endregion

            IsPianoKeyPressed = true;

            Invalidate();

            owner.OnPianoKeyDown(new PianoKeyEventArgs(noteID));
        }

        public void ReleasePianoKey()
        {
            #region Guard

            if (!IsPianoKeyPressed) return;

            #endregion

            IsPianoKeyPressed = false;

            Invalidate();

            owner.OnPianoKeyUp(new PianoKeyEventArgs(noteID));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                onBrush.Dispose();
                offBrush.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (MouseButtons == MouseButtons.Left) PressPianoKey();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsPianoKeyPressed) ReleasePianoKey();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            PressPianoKey();

            if (!owner.Focused) owner.Focus();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ReleasePianoKey();

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.X < 0 || e.X > Width || e.Y < 0 || e.Y > Height) Capture = false;

            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(IsPianoKeyPressed ? onBrush : offBrush, 0, 0, Size.Width, Size.Height);

            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Size.Width - 1, Size.Height - 1);

            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate(); // Calls OnPaint while resizing to prevent design errors
            base.OnResize(e);
        }
    }
}