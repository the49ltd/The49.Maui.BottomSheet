using Android.Views;
using Android.Widget;
using AView = Android.Views.View;
using Android.Content;

namespace The49.Maui.BottomSheet;

internal class StayOnFrontView : FrameLayout
{
    public StayOnFrontView(Context context) : base(context) {}

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        if (Parent is ViewGroup vg)
        {
            vg.ChildViewAdded += ParentChildViewAdded;
            vg.ChildViewRemoved += ParentChildViewRemoved;
        }
    }

    void ParentChildViewAdded(object sender, ViewGroup.ChildViewAddedEventArgs e)
    {
        BringToFront();
    }
    void ParentChildViewRemoved(object sender, ViewGroup.ChildViewRemovedEventArgs e)
    {
        BringToFront();
    }

    protected override void OnDetachedFromWindow()
    {
        base.OnDetachedFromWindow();
        if (Parent is ViewGroup vg)
        {
            vg.ChildViewAdded -= ParentChildViewAdded;
            vg.ChildViewRemoved -= ParentChildViewRemoved;
        }
    }

    internal void SetContentView(AView view)
    {
        RemoveAllViews();
        AddView(view);
    }
}
