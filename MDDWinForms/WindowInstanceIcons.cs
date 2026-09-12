using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MDDWinForms
{
    /// <summary>UI-thread-only window identities for handler-backed context menu targets.
    /// A single visible window keeps its original icon; multiple windows in a group get suits.
    /// Registrations and menu images are released with their controls/windows/menu items.</summary>
    public static class WindowInstanceIcons
    {
        private sealed class WindowEntry
        {
            public Form Window;
            public string Group;
            public Icon Original;
            public Icon Generated;
            public int Slot = -1;

            public void Apply(int slot)
            {
                if (slot == Slot) return;
                var next = slot < 0 ? null : CreateBadgedIcon(Original, slot);
                var previous = Generated;
                if (!Window.IsDisposed) Window.Icon = next ?? Original;
                Generated = next;
                Slot = slot;
                previous?.Dispose();
            }
        }

        private sealed class Registration
        {
            public WindowEntry Entry;
            public Control Control;
            public EventHandler Disposed;
        }

        private sealed class MenuBinding
        {
            public ToolStripMenuItem Item;
            public object Handler;
            public Image Image;

            public void Refresh()
            {
                Image next = null;
                if (registrations.TryGetValue(Handler, out var registration) && !registration.Entry.Window.IsDisposed)
                {
                    var size = Item.Owner?.ImageScalingSize ?? new Size(16, 16);
                    using (var icon = new Icon(registration.Entry.Window.Icon, size))
                        next = icon.ToBitmap();
                }
                var previous = Image;
                Item.Image = next;
                Image = next;
                previous?.Dispose();
            }
        }

        private static readonly List<WindowEntry> windows = new List<WindowEntry>();
        private static readonly Dictionary<object, Registration> registrations = new Dictionary<object, Registration>();
        private static readonly Dictionary<ToolStripMenuItem, MenuBinding> menus = new Dictionary<ToolStripMenuItem, MenuBinding>();

        /// <summary>Associate a handler with its destination control, not its ReferencingObject
        /// (which often identifies the source window). Call after the control has a parent form.</summary>
        public static void Attach(object handler, Control control, string group)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (string.IsNullOrWhiteSpace(group)) throw new ArgumentException("A window group is required.", nameof(group));
            var window = control.FindForm();
            if (window == null || window.IsDisposed || control.IsDisposed) return;
            if (registrations.TryGetValue(handler, out var existing))
            {
                if (existing.Entry.Window == window && existing.Control == control) return;
                Detach(handler);
            }
            var entry = windows.FirstOrDefault(x => x.Window == window);
            if (entry == null)
            {
                entry = new WindowEntry { Window = window, Group = group, Original = window.Icon };
                windows.Add(entry);
                window.VisibleChanged += WindowVisibleChanged;
                window.FormClosed += WindowClosed;
                window.Disposed += WindowDisposed;
            }
            // Several detail controls embedded in one form still represent one destination window.
            var registration = new Registration { Entry = entry, Control = control };
            registration.Disposed = (s, e) => Detach(handler);
            registrations.Add(handler, registration);
            control.Disposed += registration.Disposed;
            Refresh(entry.Group);
        }

        /// <summary>Bind a menu entry to the same icon as its registered destination window.
        /// The binding owns its bitmap and is removed when the menu item is disposed.</summary>
        public static void BindMenuItem(ToolStripMenuItem item, object handler)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (!menus.TryGetValue(item, out var binding))
            {
                binding = new MenuBinding { Item = item };
                menus.Add(item, binding);
                item.Disposed += MenuDisposed;
            }
            binding.Handler = handler;
            binding.Refresh();
        }

        private static void MenuDisposed(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            if (!menus.TryGetValue(item, out var binding)) return;
            menus.Remove(item);
            item.Disposed -= MenuDisposed;
            binding.Image?.Dispose();
        }

        private static void Detach(object handler)
        {
            if (!registrations.TryGetValue(handler, out var registration)) return;
            registrations.Remove(handler);
            registration.Control.Disposed -= registration.Disposed;
            var entry = registration.Entry;
            if (!registrations.Values.Any(x => x.Entry == entry)) RemoveWindow(entry);
            Refresh(entry.Group);
        }

        private static void WindowVisibleChanged(object sender, EventArgs e)
        {
            var entry = windows.FirstOrDefault(x => x.Window == sender);
            if (entry != null) Refresh(entry.Group);
        }

        private static void WindowClosed(object sender, FormClosedEventArgs e) => ForgetWindow((Form)sender);
        private static void WindowDisposed(object sender, EventArgs e) => ForgetWindow((Form)sender);

        private static void ForgetWindow(Form window)
        {
            var entry = windows.FirstOrDefault(x => x.Window == window);
            if (entry == null) return;
            foreach (var handler in registrations.Where(x => x.Value.Entry == entry).Select(x => x.Key).ToArray())
            {
                var registration = registrations[handler];
                registration.Control.Disposed -= registration.Disposed;
                registrations.Remove(handler);
            }
            RemoveWindow(entry);
            Refresh(entry.Group);
        }

        private static void RemoveWindow(WindowEntry entry)
        {
            windows.Remove(entry);
            entry.Window.VisibleChanged -= WindowVisibleChanged;
            entry.Window.FormClosed -= WindowClosed;
            entry.Window.Disposed -= WindowDisposed;
            entry.Apply(-1);
        }

        private static void Refresh(string group)
        {
            var members = windows.Where(x => x.Group == group).ToList();
            var visible = members.Where(x => !x.Window.IsDisposed && x.Window.Visible).ToList();
            foreach (var entry in members.Except(visible)) entry.Apply(-1);
            if (visible.Count < 2)
                foreach (var entry in visible) entry.Apply(-1);
            else
            {
                var used = new HashSet<int>(visible.Where(x => x.Slot >= 0).Select(x => x.Slot));
                foreach (var entry in visible.Where(x => x.Slot < 0))
                {
                    int slot = 0;
                    while (used.Contains(slot)) slot++;
                    used.Add(slot);
                    entry.Apply(slot);
                }
            }
            // Copies keep open menu items valid even after a previous window icon is disposed.
            foreach (var binding in menus.Values.ToArray()) binding.Refresh();
        }

        private static Icon CreateBadgedIcon(Icon original, int slot)
        {
            var sizes = new[] { 16, 20, 24, 32, 40, 48, 64, 128, 256 };
            var frames = new List<byte[]>();
            foreach (int size in sizes)
            using (var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                using (var baseIcon = new Icon(original, size, size))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawIcon(baseIcon, new Rectangle(0, 0, size, size));
                    DrawBadge(graphics, size, slot);
                }
                using (var memory = new MemoryStream())
                {
                    if (size == 256) bitmap.Save(memory, ImageFormat.Png);
                    else
                    using (var writer = new BinaryWriter(memory, System.Text.Encoding.UTF8, true))
                    {
                        int stride = ((size + 31) / 32) * 4;
                        writer.Write(40); writer.Write(size); writer.Write(size * 2);
                        writer.Write((ushort)1); writer.Write((ushort)32); writer.Write(0);
                        writer.Write(size * size * 4 + stride * size);
                        writer.Write(0); writer.Write(0); writer.Write(0); writer.Write(0);
                        for (int y = size - 1; y >= 0; y--)
                        for (int x = 0; x < size; x++)
                        {
                            var pixel = bitmap.GetPixel(x, y);
                            writer.Write(pixel.B); writer.Write(pixel.G); writer.Write(pixel.R); writer.Write(pixel.A);
                        }
                        for (int y = size - 1; y >= 0; y--)
                        {
                            var mask = new byte[stride];
                            for (int x = 0; x < size; x++)
                                if (bitmap.GetPixel(x, y).A == 0) mask[x / 8] |= (byte)(128 >> (x % 8));
                            writer.Write(mask);
                        }
                    }
                    frames.Add(memory.ToArray());
                }
            }
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory, System.Text.Encoding.UTF8, true))
                {
                    writer.Write((ushort)0); writer.Write((ushort)1); writer.Write((ushort)sizes.Length);
                    int offset = 6 + sizes.Length * 16;
                    for (int i = 0; i < sizes.Length; i++)
                    {
                        writer.Write((byte)(sizes[i] == 256 ? 0 : sizes[i]));
                        writer.Write((byte)(sizes[i] == 256 ? 0 : sizes[i]));
                        writer.Write((byte)0); writer.Write((byte)0);
                        writer.Write((ushort)1); writer.Write((ushort)32);
                        writer.Write(frames[i].Length); writer.Write(offset);
                        offset += frames[i].Length;
                    }
                    foreach (var frame in frames) writer.Write(frame);
                }
                memory.Position = 0;
                using (var icon = new Icon(memory)) return (Icon)icon.Clone();
            }
        }

        private static void DrawBadge(Graphics graphics, int size, int slot)
        {
            string[] suits = { "\u2666", "\u2663", "\u2665", "\u2660" };
            Color[] colors = { Color.Yellow, Color.Black, Color.Red, Color.RoyalBlue };
            string text = slot < suits.Length ? suits[slot] : (slot + 1).ToString();
            using (var family = new FontFamily("Segoe UI Symbol"))
            using (var path = new GraphicsPath())
            using (var format = StringFormat.GenericTypographic)
            {
                path.AddString(text, family, (int)FontStyle.Regular, 32, Point.Empty, format);
                var bounds = path.GetBounds();
                float margin = Math.Max(1f, size / 32f);
                float extent = size * 0.58f - margin;
                float scale = Math.Min(extent / bounds.Width, extent / bounds.Height);
                using (var transform = new Matrix(scale, 0, 0, scale,
                    size - margin * 1.5f - bounds.Width * scale - bounds.X * scale,
                    size - margin * 1.5f - bounds.Height * scale - bounds.Y * scale))
                    path.Transform(transform);
                using (var halo = new Pen(Color.White, margin * 2.5f) { LineJoin = LineJoin.Round })
                using (var outline = new Pen(Color.FromArgb(35, 45, 60), margin) { LineJoin = LineJoin.Round })
                using (var fill = new SolidBrush(slot < colors.Length ? colors[slot] : Color.Black))
                {
                    graphics.DrawPath(halo, path);
                    graphics.DrawPath(outline, path);
                    graphics.FillPath(fill, path);
                }
            }
        }
    }
}
