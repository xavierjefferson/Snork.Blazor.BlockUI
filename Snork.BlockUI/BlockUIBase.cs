using System.Collections.Specialized;
using Microsoft.AspNetCore.Components;

namespace Snork.Blazor.BlockUI
{
    public class BlockUIBase : ComponentBase
    {
        public BlockUIBase WithCss(Action<ContainerCss> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            action(_css);
            return this;
        }

        public BlockUIBase WithOverlay(Action<OverlayCss> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            action(_overlayCss);
            return this;
        }

        private OverlayCss _overlayCss = new();
        private ContainerCss _css = new();
        private System.Timers.Timer _timer = new();

        [Parameter] public RenderFragment? Template { get; set; }


        protected bool Visible { get; set; }

        [Parameter] public string? Title { get; set; }

        [Parameter] public int BaseZ { get; set; } = 1000;

        [Parameter] public bool CenterX { get; set; } = true;

        [Parameter] public bool CenterY { get; set; } = true;

        [Parameter] public bool AllowBodyStretch { get; set; } = true;

        [Parameter] public TimeSpan FadeIn { get; set; } = TimeSpan.FromMilliseconds(200);

        [Parameter] public TimeSpan FadeOut { get; set; } = TimeSpan.FromMilliseconds(400);

        [Parameter] public TimeSpan Timeout { get; set; } = TimeSpan.FromMilliseconds(0);

        [Parameter] public bool ShowOverlay { get; set; } = true;

        [Parameter] public Func<Task>? OnBlock { get; set; }

        [Parameter] public Func<Task>? OnUnblock { get; set; }

        [Parameter] public string? BlockMessageClass { get; set; } = "blockMsg";

        [Parameter] public bool IgnoreIfBlocked { get; set; } = false;


        public async Task Unblock()
        {
            Visible = false;
            _timer.Stop();
            StateHasChanged();
            if (OnUnblock != null) await OnUnblock();

            await Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            _timer.AutoReset = false;
            _timer.Elapsed += (a, b) => { InvokeAsync(Unblock); };
            await base.OnInitializedAsync();
        }

        protected RenderFragment? CurrentBody;

        public async Task Block(RenderFragment body)
        {
            CurrentBody = body;
            if (IgnoreIfBlocked && Visible) return;
            Visible = true;
            StateHasChanged();
            if (OnBlock != null) await OnBlock();
            if (Timeout.Ticks > 0)
            {
                _timer.Stop();
                _timer.Interval = Timeout.TotalMilliseconds;
                _timer.Start();
            }

            await Task.CompletedTask;
        }

        public async Task Block()
        {
            await Block(Template ?? GetDefaultMessage());

            if (IgnoreIfBlocked && Visible) return;
            Visible = true;
            StateHasChanged();
            if (OnBlock != null) await OnBlock();
            if (Timeout.Ticks > 0)
            {
                _timer.Stop();
                _timer.Interval = Timeout.TotalMilliseconds;
                _timer.Start();
            }

            await Task.CompletedTask;
        }

        private static RenderFragment GetDefaultMessage()
        {
            return builder => { builder.AddMarkupContent(1, "<h1>Please wait...</h1>"); };
        }

        protected string GetOverlayStyle()
        {
            var nameValueCollection = new NameValueCollection()
            {
                { "z-index", BaseZ.ToString() },
                { "top", "0" },
                { "position", "fixed" },
                { "left", "0" },
                { "width", "100%" },
                { "height", "100%" },
                { "cursor", _overlayCss.Cursor },
                { "background-color", _overlayCss.BackgroundColor },
                { "opacity", _overlayCss.Opacity.ToString() }
            };
            return MakeStyle(nameValueCollection);
        }

        protected string GetOuterStyle()
        {
            var nameValueCollection = new NameValueCollection()
            {
                { "z-index", (BaseZ+11).ToString() },
                { "top", "0" },
                { "position", "fixed" },
                { "left", "0" },
                { "width", "100%" },
                { "height", "100%" },
                { "background-color", "transparent"},
                { "cursor", _css.Cursor },
                
            };
            return MakeStyle(nameValueCollection);
        }

        protected string GetStyle()
        {
            var nameValueCollection = new NameValueCollection()
            {
                { "padding", _css.Padding },
                { "margin", _css.Margin },
                { "width", _css.Width },
                //{ "top", _css.Top },
                //{ "left", _css.Left },
                //{ "position", "fixed" },
                { "text-align", _css.TextAlign },
                { "color", _css.Color },
                { "border", _css.Border },
                { "background-color", _css.BackgroundColor },
                { "cursor", _css.Cursor },
                //{ "z-index", (BaseZ + 11).ToString() }
            };
            return MakeStyle(nameValueCollection);
        }

        private static string MakeStyle(NameValueCollection nameValueCollection)
        {
            return string.Join(";", nameValueCollection.AllKeys.Select(i => $"{i}:{nameValueCollection[i]}"));
        }
    }
}