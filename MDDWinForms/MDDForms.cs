using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDDFoundation;

namespace MDDWinForms
{
    public enum WindowPositions
    {
        TopLeft = 7, TopMiddle = 8, TopRight = 9, MiddleLeft = 4, MiddleMiddle = 5, MiddleRight = 6, BottomLeft = 1, BottomMiddle = 2, BottomRight = 3, FullyAuto = 0,
        
        OneOne = 11, OneTwo = 12, OneThree = 13, OneFour = 14, OneFive = 15,
        TwoOne = 21, TwoTwo = 22, TwoThree = 23, TwoFour = 24, TwoFive = 25,
        ThreeOne = 31, ThreeTwo = 32, ThreeThree = 33, ThreeFour = 34, ThreeFive = 35,
        FourOne = 41, FourTwo = 42, FourThree = 43, FourFour = 44, FourFive = 45,
        FiveOne = 51, FiveTwo = 52, FiveThree = 53, FiveFour = 54, FiveFive = 55
    }
    public enum ScreenUsage
    {
        None, Single, Dual, Triple, Quad, Nine, Sixteen, TwentyFive
    }
    public static class MDDForms
    {
        public static string InstanceQualifier { get; set; }

        private static readonly MethodInfo onValidating = typeof(Control).GetMethod("OnValidating", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo onValidated = typeof(Control).GetMethod("OnValidated", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool ManualValidate(this Control controlToValidate)
        {
            //Type t = typeof(Control);

            //MethodInfo mi = t.GetMethod("NotifyValidating", BindingFlags.Instance | BindingFlags.NonPublic);
            CancelEventArgs e = new CancelEventArgs();
            onValidating.Invoke(controlToValidate, new object[] { e });
            if (e.Cancel) return false;
            onValidated.Invoke(controlToValidate, new object[] { EventArgs.Empty });
            return true;
        }
        public static List<WindowPositions> WindowPositionsOf(ScreenUsage su)
        {
            var l = new List<WindowPositions>();
            foreach (WindowPositions item in Enum.GetValues(typeof(WindowPositions)))
            {
                switch (item)
                {
                    case WindowPositions.TopLeft:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Quad || su == ScreenUsage.Single)
                            l.Add(item);
                        break;
                    case WindowPositions.TopMiddle:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Triple)
                            l.Add(item);
                        break;
                    case WindowPositions.TopRight:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Quad)
                            l.Add(item);
                        break;
                    case WindowPositions.MiddleLeft:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Dual)
                            l.Add(item);
                        break;
                    case WindowPositions.MiddleMiddle:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Triple)
                            l.Add(item);
                        break;
                    case WindowPositions.MiddleRight:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Dual)
                            l.Add(item);
                        break;
                    case WindowPositions.BottomLeft:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Quad)
                            l.Add(item);
                        break;
                    case WindowPositions.BottomMiddle:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Triple)
                            l.Add(item);
                        break;
                    case WindowPositions.BottomRight:
                        if (su == ScreenUsage.Nine || su == ScreenUsage.Quad)
                            l.Add(item);
                        break;
                    case WindowPositions.OneOne:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item);
                        break;
                    case WindowPositions.OneTwo:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.OneThree:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.OneFour:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.OneFive:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.TwoOne:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.TwoTwo:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.TwoThree:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.TwoFour:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.TwoFive:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.ThreeOne:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.ThreeTwo:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.ThreeThree:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.ThreeFour:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.ThreeFive:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FourOne:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FourTwo:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FourThree:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FourFour:
                        if (su == ScreenUsage.Sixteen || su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FourFive:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FiveOne:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FiveTwo:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FiveThree:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FiveFour:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FiveFive:
                        if (su == ScreenUsage.TwentyFive)
                            l.Add(item); 
                        break;
                    case WindowPositions.FullyAuto:
                        break;
                    default:
                        break;
                }
            }
            return l;
        }
        public static Rectangle GetConfigRectangle(Rectangle AccordingTo, WindowPositions InWindowPosition, ScreenUsage InScreenUsage, int Buffer, Point InLocation)
        {
            var r = new Rectangle();

            double height2 = AccordingTo.Height / 2.0;
            double width2 = AccordingTo.Width / 2.0;
            double height3 = AccordingTo.Height / 3.0;
            double width3 = AccordingTo.Width / 3.0;

            if (InWindowPosition != WindowPositions.FullyAuto && !WindowPositionsOf(InScreenUsage).Contains(InWindowPosition))
                throw new ArgumentException($"WindowPostion {InWindowPosition} is invalid for ScreenUsage {InScreenUsage}");

            switch (InScreenUsage)
            {
                case ScreenUsage.None:
                    break;
                case ScreenUsage.Single:
                    r.Y = AccordingTo.Top + Buffer;
                    r.X = AccordingTo.Left + Buffer;
                    r.Width = AccordingTo.Width - (Buffer * 2);
                    r.Height = AccordingTo.Height - (Buffer * 2);
                    break;
                case ScreenUsage.Dual:
                    if (InWindowPosition == WindowPositions.FullyAuto)
                    {
                        if (AccordingTo.Width > AccordingTo.Height)
                        {
                            if (InLocation.X < AccordingTo.Left + width2)
                            {
                                if (InLocation.Y < height3)
                                    InWindowPosition = WindowPositions.TopLeft;
                                else if (InLocation.Y > (2 * height3))
                                    InWindowPosition = WindowPositions.BottomLeft;
                                else InWindowPosition = WindowPositions.MiddleLeft;
                            }
                            else
                            {
                                if (InLocation.Y < height3)
                                    InWindowPosition = WindowPositions.TopRight;
                                else if (InLocation.Y > (2 * height3))
                                    InWindowPosition = WindowPositions.BottomRight;
                                else InWindowPosition = WindowPositions.MiddleRight;
                            }
                        }
                        else
                        {
                            if (InLocation.Y < AccordingTo.Top + height2)
                                InWindowPosition = WindowPositions.TopMiddle;
                            else
                                InWindowPosition = WindowPositions.BottomMiddle;
                        }
                    }
                    //if (InWindowPosition == WindowPositions.TopLeft)
                    //    if (AccordingTo.Width > AccordingTo.Height) { InWindowPosition = WindowPositions.MiddleLeft; } else { InWindowPosition = WindowPositions.TopMiddle; }
                    //if (InWindowPosition == WindowPositions.BottomRight)
                    //    if (AccordingTo.Width > AccordingTo.Height) { InWindowPosition = WindowPositions.MiddleRight; } else { InWindowPosition = WindowPositions.BottomMiddle; }
                    switch (InWindowPosition)
                    {
                        case WindowPositions.TopMiddle:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = AccordingTo.Width - (Buffer * 2);
                            r.Height = Convert.ToInt32(Math.Ceiling(height2 - (Buffer * 2)));
                            break;
                        case WindowPositions.MiddleLeft:
                        case WindowPositions.MiddleMiddle:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2);
                            break;
                        case WindowPositions.BottomLeft:
                            r.Y = AccordingTo.Top + Buffer + Convert.ToInt32(Math.Ceiling(height3));
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2) - Convert.ToInt32(Math.Ceiling(height3));
                            break;
                        case WindowPositions.TopLeft:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2) - Convert.ToInt32(Math.Ceiling(height3));
                            break;
                        case WindowPositions.MiddleRight:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width2 + Buffer));
                            r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2);
                            break;
                        case WindowPositions.BottomRight:
                            r.Y = AccordingTo.Top + Buffer + Convert.ToInt32(Math.Ceiling(height3));
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width2 + Buffer));
                            r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2) - Convert.ToInt32(Math.Ceiling(height3));
                            break;
                        case WindowPositions.TopRight:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width2 + Buffer));
                            r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2) - Convert.ToInt32(Math.Ceiling(height3));
                            break;
                        case WindowPositions.BottomMiddle:
                            r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + height2 + Buffer));
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = AccordingTo.Width - (Buffer * 2);
                            r.Height = Convert.ToInt32(Math.Ceiling(height2 - (Buffer * 2)));
                            break;
                        default:
                            break;
                    }
                    break;
                case ScreenUsage.Triple:
                    if (InWindowPosition == WindowPositions.FullyAuto)
                    {
                        if (AccordingTo.Width > AccordingTo.Height)
                        {
                            if (InLocation.X < AccordingTo.Left + width3)
                                InWindowPosition = WindowPositions.MiddleLeft;
                            else if (InLocation.X < AccordingTo.Left + 2 * width3)
                                InWindowPosition = WindowPositions.MiddleMiddle;
                            else
                                InWindowPosition = WindowPositions.MiddleRight;
                        }
                        else
                        {
                            if (InLocation.Y < AccordingTo.Top + height3)
                                InWindowPosition = WindowPositions.TopMiddle;
                            else if (InLocation.Y < AccordingTo.Top + 2 * height3)
                                InWindowPosition = WindowPositions.MiddleMiddle;
                            else
                                InWindowPosition = WindowPositions.BottomMiddle;
                        }
                    }
                    if (InWindowPosition == WindowPositions.TopLeft)
                        if (AccordingTo.Width > AccordingTo.Height) { InWindowPosition = WindowPositions.MiddleLeft; } else { InWindowPosition = WindowPositions.TopMiddle; }
                    if (InWindowPosition == WindowPositions.BottomRight)
                        if (AccordingTo.Width > AccordingTo.Height) { InWindowPosition = WindowPositions.MiddleRight; } else { InWindowPosition = WindowPositions.BottomMiddle; }
                    switch (InWindowPosition)
                    {
                        case WindowPositions.TopMiddle:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = AccordingTo.Width - (Buffer * 2);
                            r.Height = Convert.ToInt32(Math.Ceiling(height3 - (Buffer * 2)));
                            break;
                        case WindowPositions.MiddleLeft:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = Convert.ToInt32(Math.Ceiling(width3 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2);
                            break;
                        case WindowPositions.MiddleMiddle:
                            if (AccordingTo.Width > AccordingTo.Height)
                            {
                                r.Y = AccordingTo.Top + Buffer;
                                r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width3 + Buffer));
                                r.Width = Convert.ToInt32(Math.Ceiling(width3 - (Buffer * 2)));
                                r.Height = AccordingTo.Height - (Buffer * 2);
                            }
                            else
                            {
                                r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + height3 + Buffer));
                                r.X = AccordingTo.Left + Buffer;
                                r.Width = AccordingTo.Width - (Buffer * 2);
                                r.Height = Convert.ToInt32(Math.Ceiling(height3 - (Buffer * 2)));
                            }
                            break;
                        case WindowPositions.MiddleRight:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + (2 * width3) + Buffer));
                            r.Width = Convert.ToInt32(Math.Ceiling(width3 - (Buffer * 2)));
                            r.Height = AccordingTo.Height - (Buffer * 2);
                            break;
                        case WindowPositions.BottomMiddle:
                            r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + (2 * height3) + Buffer));
                            r.X = AccordingTo.Left + Buffer;
                            r.Width = AccordingTo.Width - (Buffer * 2);
                            r.Height = Convert.ToInt32(Math.Ceiling(height3 - (Buffer * 2)));
                            break;
                        default:
                            break;
                    }
                    break;
                case ScreenUsage.Quad:
                    if (InWindowPosition == WindowPositions.FullyAuto)
                    {
                        if (InLocation.X < AccordingTo.Left + width2 && InLocation.Y < AccordingTo.Top + height2)
                            InWindowPosition = WindowPositions.TopLeft;
                        else if (InLocation.X > AccordingTo.Left + width2 && InLocation.Y < AccordingTo.Top + height2)
                            InWindowPosition = WindowPositions.TopRight;
                        else if (InLocation.X < AccordingTo.Left + width2)
                            InWindowPosition = WindowPositions.BottomLeft;
                        else
                            InWindowPosition = WindowPositions.BottomRight;
                    }
                    switch (InWindowPosition)
                    {
                        case WindowPositions.TopLeft:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = AccordingTo.Left + Buffer;
                            break;
                        case WindowPositions.TopRight:
                            r.Y = AccordingTo.Top + Buffer;
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width2 + Buffer));
                            break;
                        case WindowPositions.BottomLeft:
                            r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + height2 + Buffer));
                            r.X = AccordingTo.Left + Buffer;
                            break;
                        case WindowPositions.BottomRight:
                            r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + height2 + Buffer));
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width2 + Buffer));
                            break;
                        default:
                            break;
                    }
                    r.Width = Convert.ToInt32(Math.Ceiling(width2 - (Buffer * 2)));
                    r.Height = Convert.ToInt32(Math.Ceiling(height2 - (Buffer * 2)));
                    break;
                case ScreenUsage.Nine:
                    if (InWindowPosition == WindowPositions.FullyAuto)
                    {
                        if (InLocation.X < AccordingTo.Left + width3 && InLocation.Y < AccordingTo.Top + height3)
                            InWindowPosition = WindowPositions.TopLeft;
                        else if (InLocation.X < AccordingTo.Left + width3 && InLocation.Y < AccordingTo.Top + 2 * height3)
                            InWindowPosition = WindowPositions.MiddleLeft;
                        else if (InLocation.X < AccordingTo.Left + width3)
                            InWindowPosition = WindowPositions.BottomLeft;
                        else if (InLocation.X < AccordingTo.Left + 2 * width3 && InLocation.Y < AccordingTo.Top + height3)
                            InWindowPosition = WindowPositions.TopMiddle;
                        else if (InLocation.X < AccordingTo.Left + 2 * width3 && InLocation.Y < AccordingTo.Top + 2 * height3)
                            InWindowPosition = WindowPositions.MiddleMiddle;
                        else if (InLocation.X < AccordingTo.Left + 2 * width3)
                            InWindowPosition = WindowPositions.BottomMiddle;
                        else if (InLocation.Y < AccordingTo.Top + height3)
                            InWindowPosition = WindowPositions.TopRight;
                        else if (InLocation.Y < AccordingTo.Top + 2 * height3)
                            InWindowPosition = WindowPositions.MiddleRight;
                        else
                            InWindowPosition = WindowPositions.BottomRight;
                    }
                    switch (InWindowPosition)
                    {
                        case WindowPositions.TopLeft:
                        case WindowPositions.TopMiddle:
                        case WindowPositions.TopRight:
                            r.Y = AccordingTo.Top + Buffer;
                            break;
                        case WindowPositions.MiddleLeft:
                        case WindowPositions.MiddleMiddle:
                        case WindowPositions.MiddleRight:
                            r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + height3 + Buffer));
                            break;
                        case WindowPositions.BottomLeft:
                        case WindowPositions.BottomMiddle:
                        case WindowPositions.BottomRight:
                            r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + (2 * height3) + Buffer));
                            break;
                        default:
                            break;
                    }
                    switch (InWindowPosition)
                    {
                        case WindowPositions.TopLeft:
                        case WindowPositions.MiddleLeft:
                        case WindowPositions.BottomLeft:
                            r.X = AccordingTo.Left + Buffer;
                            break;
                        case WindowPositions.TopMiddle:
                        case WindowPositions.MiddleMiddle:
                        case WindowPositions.BottomMiddle:
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + width3 + Buffer));
                            break;
                        case WindowPositions.TopRight:
                        case WindowPositions.MiddleRight:
                        case WindowPositions.BottomRight:
                            r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + (2 * width3) + Buffer));
                            break;
                        default:
                            break;
                    }
                    r.Width = Convert.ToInt32(Math.Ceiling(width3 - (Buffer * 2)));
                    r.Height = Convert.ToInt32(Math.Ceiling(height3 - (Buffer * 2)));
                    break;
                case ScreenUsage.Sixteen:
                    double width4 = AccordingTo.Width / 4.0;
                    double height4 = AccordingTo.Height / 4.0;
                    if (InWindowPosition == WindowPositions.FullyAuto)
                    {
                        int RelX = InLocation.X - AccordingTo.Left;
                        int RelY = InLocation.Y - AccordingTo.Top;

                        int tmpWindowPosition = (Convert.ToInt32(Math.Floor(Convert.ToDouble(RelY) / Convert.ToDouble(AccordingTo.Height) * 4)) + 1) * 10;
                        tmpWindowPosition = tmpWindowPosition + Convert.ToInt32(Math.Floor(Convert.ToDouble(RelX) / Convert.ToDouble(AccordingTo.Width) * 4)) + 1;
                        InWindowPosition = (WindowPositions)tmpWindowPosition;
                    }
                    int YMultFactor = (int)InWindowPosition / 10 - 1;
                    int XMultFactor = (int)InWindowPosition % 10 - 1;

                    r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + (XMultFactor * width4) + Buffer));
                    r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + (YMultFactor * height4) + Buffer));

                    r.Width = Convert.ToInt32(Math.Ceiling(width4 - (Buffer * 2)));
                    r.Height = Convert.ToInt32(Math.Ceiling(height4 - (Buffer * 2)));
                    break;
                case ScreenUsage.TwentyFive:
                    double width5 = AccordingTo.Width / 5.0;
                    double height5 = AccordingTo.Height / 5.0;
                    if (InWindowPosition == WindowPositions.FullyAuto)
                    {
                        int RelX = InLocation.X - AccordingTo.Left;
                        int RelY = InLocation.Y - AccordingTo.Top;

                        int tmpWindowPosition = (Convert.ToInt32(Math.Floor(Convert.ToDouble(RelY) / Convert.ToDouble(AccordingTo.Height) * 5)) + 1) * 10;
                        tmpWindowPosition = tmpWindowPosition + Convert.ToInt32(Math.Floor(Convert.ToDouble(RelX) / Convert.ToDouble(AccordingTo.Width) * 5)) + 1;
                        InWindowPosition = (WindowPositions)tmpWindowPosition;
                    }
                    YMultFactor = (int)InWindowPosition / 10 - 1;
                    XMultFactor = (int)InWindowPosition % 10 - 1;

                    r.X = Convert.ToInt32(Math.Ceiling(AccordingTo.Left + (XMultFactor * width5) + Buffer));
                    r.Y = Convert.ToInt32(Math.Ceiling(AccordingTo.Top + (YMultFactor * height5) + Buffer));

                    r.Width = Convert.ToInt32(Math.Ceiling(width5 - (Buffer * 2)));
                    r.Height = Convert.ToInt32(Math.Ceiling(height5 - (Buffer * 2)));
                    break;
                default:
                    break;
            }
            return r;
        }

        public static void ShowInstance(this Form frm)
        {
            var contains = false;
            foreach (var f in Application.OpenForms)
            {
                if (frm.Equals(f))
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                if (!string.IsNullOrWhiteSpace(InstanceQualifier)) frm.Text = $"{InstanceQualifier} {frm.Text}";
            }
            frm.Show();
        }
        public static Form ToForm(this Control ctl, string title = null)
        {
            var f = new ControlForm(ctl,title);
            //if (!string.IsNullOrWhiteSpace(title)) { f.Text = title; }
            //f.FormClosing += F_FormClosing;
            //f.Size = new Size(ctl.Width + 10, ctl.Height + 50);
            //ctl.Dock = DockStyle.Fill;
            //f.Controls.Add(ctl);

            // Reflection: Check for public property "OKButton" of type Button
            //var okButtonProp = ctl.GetType().GetProperty("OKButton", BindingFlags.Public | BindingFlags.Instance);
            //if (okButtonProp != null)
            //{
            //    var okButton = okButtonProp.GetValue(ctl) as Button;
            //    if (okButton != null)
            //    {
            //        f.AcceptButton = okButton;
            //    }
            //}

            return f;
        }
        public static Form ToDialogForm(this Control ctl, string title = null)
        {
            var form = new Form();
            if (!string.IsNullOrWhiteSpace(title)) { form.Text = title; }
            form.FormClosing += F_FormClosing;
            form.Size = new Size(ctl.Width + 30, ctl.Height + 100);

            var tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            var panel = new Panel
            {
                Dock = DockStyle.Fill
            };
            ctl.Dock = DockStyle.Fill;
            panel.Controls.Add(ctl);
            tableLayoutPanel.Controls.Add(panel, 0, 0);

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Size = new Size(75, 23)
            };
            buttonPanel.Controls.Add(btnCancel);

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Size = new Size(75, 23)
            };
            buttonPanel.Controls.Add(btnOk);

            tableLayoutPanel.Controls.Add(buttonPanel, 0, 1);

            form.Controls.Add(tableLayoutPanel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form;
        }

        private static void SetWaitCursor(Control control, bool useWait)
        {
            control.Cursor = useWait ? Cursors.WaitCursor : Cursors.Default;
            //if (Debugger.IsAttached) Console.WriteLine($"MDDForms.SetWaitCursor: {control.Name}");
            foreach (Control child in control.Controls)
            {
                SetWaitCursor(child, useWait);
            }
        }
        public static Form SetWaitCursor(this Control ctl)
        {
            return ctl.SynchronizedInvoke(() =>
            {
                var form = ctl.FindForm();
                if (form == null) return null;

                if (form.Cursor != Cursors.WaitCursor)
                {
                    SetWaitCursor(form, true);
                    return form;
                }

                return null;
            });
        }
        public static void ResetWaitCursor(this Form form)
        {
            form.SynchronizedInvoke(() => SetWaitCursor(form, false));
        }


        private static void F_FormClosing(object sender, FormClosingEventArgs e)
        {
            var btn = (sender as Form).Controls[0].Controls["btnOK"] as Button;
            if (btn != null && btn.DialogResult == DialogResult.Abort)
                e.Cancel = true;
        }

        public static T SingleInstanceForm<T>() where T : Form, new()
        {
            return Application.OpenForms.OfType<T>().FirstOrDefault() ?? new T();
        }
        public static T FindInstance<T>() where T : Form
        {
            var f = Application.OpenForms.OfType<T>().FirstOrDefault();
            if (f == null || f.IsDisposed) 
                return null;
            else
                return f;
        }
        public static T GetInstance<T>() where T : Form, new()
        {
            var f = Application.OpenForms.OfType<T>().FirstOrDefault();
            if (f == null || f.IsDisposed)
                return new T();
            else
                return f;
        }
        public static Form GetInstance(this Form frm, Form OtherThan = null)
        {
            foreach (Form lfrm in Application.OpenForms)
            {
                if (lfrm.GetType() == frm.GetType() && lfrm.Visible && lfrm != OtherThan)
                    return lfrm;
            }
            return (Form)frm.GetType().GetConstructor(new Type[] { }).Invoke(new object[] { });
        }
        public static void SetColumns(this DataGridView grid, object sample)
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
            if (sample != null)
            foreach (var item in sample.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var col = new DataGridViewTextBoxColumn();
                col.Name = item.Name;
                col.DataPropertyName = item.Name;
                grid.Columns.Add(col);
            }
        }
        [DllImport("Shcore.dll")]
        public static extern IntPtr SetProcessDpiAwareness([In] DpiAwareness value);
        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern IntPtr GetProcessDpiAwareness(IntPtr hprocess, out DpiAwareness value);



        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = Convert.ToSingle(size.Width) / Convert.ToSingle(originalWidth);
                float percentHeight = Convert.ToSingle(size.Height) / Convert.ToSingle(originalHeight);
                float percent;
                if (percentHeight < percentWidth)
                    percent = percentHeight;
                else
                    percent = percentWidth;
                newWidth = Convert.ToInt32(originalWidth * percent);
                newHeight = Convert.ToInt32(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }

            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        public static void ResizeImage(ref Image image, Size size, bool preserveAspectRatio = true)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            int newWidth, newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / originalWidth;
                float percentHeight = (float)size.Height / originalHeight;
                float percent = Math.Min(percentWidth, percentHeight);
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }

            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            image.Dispose(); // Dispose the old image
            image = newImage; // Replace with the new image
        }
        public static byte[] ToByteArray(this Image imageIn)
        {
            //using (var ms = new MemoryStream())
            //{
            //    imageIn.Save(ms, imageIn.RawFormat);
            //    return ms.ToArray();
            //}
            var ic = new ImageConverter();
            byte[] bytes = (byte[])ic.ConvertTo(imageIn, typeof(byte[]));
            return bytes;
        }
        public static byte[] ToByteArray(this Image imageIn, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, format);
                return ms.ToArray();
            }
        }
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null) return null;
            using (var ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }
    }
    public enum DpiAwareness
    {
        Unaware = 0,
        DPIAware = 1,
        PerMonitorDPIAware = 2
    }
}
