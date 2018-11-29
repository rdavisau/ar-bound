using System;
using Foundation;
using UIKit;

namespace ARKitMeetup.Helpers
{
    public class InlineTableViewSourceWithoutRowHeight : UITableViewSource, IUITableViewDataSource, IUITableViewDelegate
    {
        public Action<UITableView, NSIndexPath> _AccessoryButtonTapped { get; set; }
        public Func<UITableView, NSIndexPath, UITableViewCellAccessory> _AccessoryForRow { get; set; }

        public Func<UITableView, NSIndexPath, Boolean> _CanEditRow { get; set; }
        public Func<UITableView, NSIndexPath, Boolean> _CanMoveRow { get; set; }

        public Func<UITableView, NSIndexPath, UITableViewCellEditingStyle> _EditingStyleForRow { get; set; }
        public Action<UITableView, UITableViewCellEditingStyle, NSIndexPath> _CommitEditingStyle { get; set; }
        public Action<UITableView, NSIndexPath> _DidEndEditing { get; set; }
        public Func<UITableView, NSIndexPath, UITableViewRowAction[]> _EditActionsForRow { get; set; }

        public Func<UITableView, nint> _NumberOfSections { get; set; }
        public Func<UITableView, int, nint> _RowsInSection { get; set; }
        public Func<UITableView, NSIndexPath, UITableViewCell> _GetCell { get; set; }

        public Func<UITableView, NSIndexPath, nint> _IndentationLevel { get; set; }

        public Func<UITableView, NSIndexPath, NSIndexPath> _WillSelectRow { get; set; }
        public Action<UITableView, NSIndexPath> _RowDeselected { get; set; }
        public Action<UITableView, NSIndexPath> _RowSelected { get; set; }

        public Func<UITableView, NSIndexPath, Boolean> _ShouldHighlightRow { get; set; }
        public Func<UITableView, NSIndexPath, Boolean> _ShouldIndentWhileEditing { get; set; }

        public Func<UITableView, NSIndexPath, String> _TitleForDeleteConfirmation { get; set; }
        public Func<UITableView, nint, String> _TitleForFooter { get; set; }
        public Func<UITableView, nint, String> _TitleForHeader { get; set; }

        public Action<UITableView, UITableViewCell, NSIndexPath> _WillDisplay { get; set; }
        public Action<UITableView, UIView, int> _WillDisplayHeaderView { get; set; }
        public Action<UITableView, UIView, int> _WillDisplayFooterView { get; set; }

        public Func<UITableView, nint, UIView> _ViewForHeader { get; set; }
        public Func<UITableView, nint, UIView> _ViewForFooter { get; set; }

        public Func<UITableView, nint, nfloat> _HeightForHeader { get; set; }
        public Func<UITableView, nint, nfloat> _HeightForFooter { get; set; }

        public Action<UIScrollView> _DraggingStarted { get; set; }
        public Action<UIScrollView, bool> _DraggingEnded { get; set; }
        public Action<UIScrollView> _DeaccelerationEnded { get; set; }
        public Action<UIScrollView> _Scrolled { get; set; }
        public Action<UIScrollView> _ScrollAnimationEnded { get; set; }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (_Scrolled != null)
                _Scrolled(scrollView);
        }

        public override void ScrollAnimationEnded(UIScrollView scrollView)
        {
            if (_ScrollAnimationEnded != null)
                _ScrollAnimationEnded(scrollView);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            if (_ViewForHeader != null)
                return _ViewForHeader(tableView, section);
            else
                return null;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            if (_HeightForFooter != null)
                return _HeightForFooter(tableView, section);
            else
                return 0f;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (_HeightForHeader != null)
                return _HeightForHeader(tableView, section);
            else
                return 0f;
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            if (_ViewForFooter != null)
                return _ViewForFooter(tableView, section);
            else
                return null;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_WillDisplay != null)
                _WillDisplay(tableView, cell, indexPath);
        }

        public override void WillDisplayHeaderView(UITableView tableView, UIView headerView, nint section)
        {
            if (_WillDisplayHeaderView != null)
                _WillDisplayHeaderView(tableView, headerView, (int)section);
        }

        public override void WillDisplayFooterView(UITableView tableView, UIView footerView, nint section)
        {
            if (_WillDisplayFooterView != null)
                _WillDisplayFooterView(tableView, footerView, (int)section);
        }

        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            if (_AccessoryButtonTapped != null)
                _AccessoryButtonTapped(tableView, indexPath);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle,
            NSIndexPath indexPath)
        {
            if (_CommitEditingStyle != null)
                _CommitEditingStyle(tableView, editingStyle, indexPath);
        }

        public override void DidEndEditing(UITableView tableView, NSIndexPath indexPath)
        {
            if (_DidEndEditing != null)
                _DidEndEditing(tableView, indexPath);
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (_EditingStyleForRow != null)
                return _EditingStyleForRow(tableView, indexPath);
            else
                return UITableViewCellEditingStyle.None;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (_GetCell != null)
                return _GetCell(tableView, indexPath);
            else
                throw new NotImplementedException("GetCell");
        }

        public override nint IndentationLevel(UITableView tableView, NSIndexPath indexPath)
        {
            if (_IndentationLevel != null)
                return _IndentationLevel(tableView, indexPath);
            else
                return 0;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (_NumberOfSections != null)
                return _NumberOfSections(tableView);
            else
                return 1;
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            if (_DraggingStarted != null)
                _DraggingStarted(scrollView);
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (_DraggingEnded != null)
                _DraggingEnded(scrollView, willDecelerate);
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            if (_DeaccelerationEnded != null)
                _DeaccelerationEnded(scrollView);
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_RowDeselected != null)
                _RowDeselected(tableView, indexPath);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_RowSelected != null)
                _RowSelected(tableView, indexPath);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_RowsInSection != null)
                return _RowsInSection(tableview, (int)section);
            else
                throw new NotImplementedException("RowsInSection");
        }

        public override Boolean ShouldHighlightRow(UITableView tableView, NSIndexPath rowIndexPath)
        {
            if (_ShouldHighlightRow != null)
                return _ShouldHighlightRow(tableView, rowIndexPath);
            else
                return true;
        }

        public override Boolean ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
        {
            if (_ShouldIndentWhileEditing != null)
                return _ShouldIndentWhileEditing(tableView, indexPath);
            else
                return true;
        }

        public override String TitleForFooter(UITableView tableView, nint section)
        {
            if (_TitleForFooter != null)
                return _TitleForFooter(tableView, section);
            else
                return null;
        }

        public override String TitleForHeader(UITableView tableView, nint section)
        {
            if (_TitleForHeader != null)
                return _TitleForHeader(tableView, section);
            else
                return null;
        }

        public override NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (_WillSelectRow != null)
                return _WillSelectRow(tableView, indexPath);
            else
                return indexPath;
        }

        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
        {
            if (_TitleForDeleteConfirmation != null)
                return _TitleForDeleteConfirmation(tableView, indexPath);
            else
                return "Delete";
        }
    }
}
