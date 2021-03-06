﻿// Developer Express Code Central Example:
// GridView descendant allows using RepositoryItems to display/edit Row Preview
// 
// This example shows how to create a GridView
// (ms-help://MS.VSCC.v90/MS.VSIPCC.v90/DevExpress.NETv9.2/DevExpress.XtraGrid/clsDevExpressXtraGridViewsGridGridViewtopic.htm)
// descendant, which will allow using a specific repository item for displaying and
// editing data in a row preview section.
// 
// 
// See Also:
// <kblink id = "K18341"/>
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E2002

// Developer Express Code Central Example:
// How to create a GridView descendant, which will allow using a specific repository item for displaying and editing data in a row preview section
// 
// This example shows how to create a GridView
// (ms-help://MS.VSCC.v90/MS.VSIPCC.v90/DevExpress.NETv9.2/DevExpress.XtraGrid/clsDevExpressXtraGridViewsGridGridViewtopic.htm)
// descendant, which will allow using a specific repository item for displaying and
// editing data in a row preview section.
// 
// 
// See Also:
// <kblink id = "K18341"/>
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E2002

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Registrator;
using System.ComponentModel;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.Handler;

namespace CustomGrid_PreviewRow
{
    public class CustomGridView : AdvBandedGridView
    {
        protected RepositoryItem fRowPreviewEdit;
        bool isRowPreviewSelected;
        int postingEditorValue;
        public CustomGridView()
            : base()
        {
            fRowPreviewEdit = null;
            isRowPreviewSelected = false;
        }

      
        protected virtual int PreviewFieldHandle
        {
            get
            {

                int previewFieldHandle = DataController.Columns.GetColumnIndex(PreviewFieldName);
                if (previewFieldHandle == -1) previewFieldHandle = -2;
                return previewFieldHandle;
            }
        }



        protected internal virtual void SetGridControlAccessMetod(GridControl newControl) { SetGridControl(newControl); }
        protected override string ViewName { get { return "CustomGridView"; } }
        [Category("Appearance"), Description("Gets or sets the repository item specifying the editor used to show row preview."), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnEditConverter, " + AssemblyInfo.SRAssemblyGridDesign),
        Editor("DevExpress.XtraGrid.Design.ColumnEditEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
        public DevExpress.XtraEditors.Repository.RepositoryItem PreviewRowEdit
        {
            get { return fRowPreviewEdit; }
            set
            {
                if (PreviewRowEdit != value)
                {
                    DevExpress.XtraEditors.Repository.RepositoryItem old = fRowPreviewEdit;
                    fRowPreviewEdit = value;
                    CustomGridViewInfo vi = ViewInfo as CustomGridViewInfo;
                    if (vi != null) vi.UpdateRowPreviewEdit(fRowPreviewEdit);
                    if (old != null) old.Disconnect(this);
                    if (PreviewRowEdit != null)
                    {
                        PreviewRowEdit.Connect(this);
                    }
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (PreviewRowEdit != null)
            {
                PreviewRowEdit.Disconnect(this);
                this.fRowPreviewEdit = null;
            }
            base.Dispose(disposing);
        }
        public virtual object GetRowPreviewValue(int rowHandle)
        {
            object result = null;
            if (PreviewFieldName.Length != 0 && DataController.IsReady)
                result = DataController.GetRowValue(rowHandle, PreviewFieldHandle);
            if (result is string) return GetRowPreviewDisplayText(rowHandle);
            return result;
        }
        protected virtual internal bool IsRowPreviewSelected
        {
            get { return isRowPreviewSelected; }
            set
            {
                if (IsRowPreviewSelected != value)
                {
                    isRowPreviewSelected = value;
                    InvalidateRow(FocusedRowHandle);
                }
            }
        }
        protected virtual internal void ActivatePreviewEditor()
        {
            if (PreviewRowEdit == null) return;
            GridDataRowInfo ri = ViewInfo.GetGridRowInfo(FocusedRowHandle) as GridDataRowInfo;
            if (ri == null) return;
            Rectangle bounds = ((CustomGridViewInfo)ViewInfo).GetRowPreviewEditBounds(ri);
            bounds.Offset(ri.PreviewBounds.Location);
            UpdateEditor(PreviewRowEdit, new UpdateEditorInfoArgs(
                false, bounds, ri.AppearancePreview, GetRowPreviewValue(FocusedRowHandle), ElementsLookAndFeel, string.Empty, null));
        }

        protected virtual internal int RaiseMeasurePreviewHeightAccessMetod(int rowHandle) { return base.RaiseMeasurePreviewHeight(rowHandle); }
   
        protected override bool PostEditor(bool causeValidation)
        {
            bool result = base.PostEditor(causeValidation);
            if (PreviewRowEdit == null) return result;
            if (this.postingEditorValue != 0) return result;
            try
            {
                this.postingEditorValue++;
                if (ActiveEditor == null || !EditingValueModified || this.fEditingCell != null) return result;
                if (causeValidation && !ValidateEditor()) return false;
                SetRowPreviewValueCore(FocusedRowHandle, EditingValue);
            }
            finally
            {
                this.postingEditorValue--;
            }
            return result;
        }
        protected override void CloseEditor(bool causeValidation)
        {
            IsRowPreviewSelected = false;
            base.CloseEditor(causeValidation);
        }
        private void SetRowPreviewValueCore(int rowHandle, object value)
        {
            if (PreviewRowEdit == null || FocusedColumn != null) return;
            try
            {
                DataController.SetRowValue(rowHandle, PreviewFieldHandle, value);
                UpdateRowAutoHeight(rowHandle);
                if (rowHandle == FocusedRowHandle && FocusedColumn == null)
                {
                    RefreshEditor(true);
                    SetFocusedRowModified();
                }
                Invalidate();
            }
            catch { }
        }
        protected override bool IsAutoHeight { get { if (PreviewRowEdit != null) return true; else return base.IsAutoHeight; } }
        public override GridColumn FocusedColumn
        {
            get { return base.FocusedColumn; }
            set
            {
                base.FocusedColumn = value;
                if (FocusedColumn != null) IsRowPreviewSelected = false;
            }
        }
    }
   [ToolboxItem(true)]
    public class CustomGridControl : GridControl
    {
        public CustomGridControl() : base() { }
        protected override void RegisterAvailableViewsCore(InfoCollection collection)
        {
            base.RegisterAvailableViewsCore(collection);
            collection.Add(new CustomGridInfoRegistrator());
        }
        protected override BaseView CreateDefaultView() { return CreateView("CustomGridView"); }
    }
    public class CustomGridPainter : BandedGridPainter
    {
        public CustomGridPainter(AdvBandedGridView view) : base(view) { }
        public virtual new CustomGridView View { get { return (CustomGridView)base.View; } }
        protected override void DrawRowPreview(GridViewDrawArgs e, GridDataRowInfo ri)
        {
          
            RepositoryItem item = ((CustomGridView)e.ViewInfo.View).PreviewRowEdit;
            if (item == null)
                base.DrawRowPreview(e, ri);
            else
                DrawRowPreviewEditor(e, ri, item);
        }
        private void DrawRowPreviewEditor(GridViewDrawArgs e, GridDataRowInfo ri, RepositoryItem item)
        {
            GridCellInfo info = new GridCellInfo(null, ri, ri.PreviewBounds);
            info.Editor = item;
            DrawCellEdit(e, ((CustomGridViewInfo)e.ViewInfo).GetRowPreviewViewInfo(e, ri), info, ri.AppearancePreview, false);
        }
    }
    public class CustomGridViewInfo : AdvBandedGridViewInfo
    {
        protected override void CalcMinBandRowHeight(int bandHeight)
        {
            if (Painter != null)
            base.CalcMinBandRowHeight(bandHeight);
        }
        
        BaseEditViewInfo fRowPreviewViewInfo;
        public CustomGridViewInfo(AdvBandedGridView gridView)
            : base(gridView)
        {
            UpdateRowPreviewEdit(View.PreviewRowEdit);
        }
        public virtual new CustomGridView View { get { return base.View as CustomGridView; } }
        public virtual void UpdateRowPreviewEdit(RepositoryItem item)
        {
            if (item != null)
                fRowPreviewViewInfo = CreateRowPreviewViewInfo(item);
            else
                fRowPreviewViewInfo = null;
        }
        protected virtual BaseEditViewInfo CreateRowPreviewViewInfo(RepositoryItem item)
        {
            BaseEditViewInfo info = item.CreateViewInfo();
            UpdateEditViewInfo(info);
            Graphics g = GInfo.AddGraphics(null);
            try
            {
                info.CalcViewInfo(g);
            }
            finally
            {
                GInfo.ReleaseGraphics();
            }
            return info;
        }
        public virtual BaseEditViewInfo GetRowPreviewViewInfo(GridViewDrawArgs e, GridDataRowInfo ri)
        {
            fRowPreviewViewInfo.Bounds = GetRowPreviewEditBounds(ri);
            fRowPreviewViewInfo.EditValue = View.GetRowPreviewValue(ri.RowHandle);
            fRowPreviewViewInfo.Focused = true;
            fRowPreviewViewInfo.CalcViewInfo(e.Graphics);
            return fRowPreviewViewInfo;
        }
        public virtual Rectangle GetRowPreviewEditBounds(GridDataRowInfo ri)
        {
            Rectangle r = new Rectangle(new Point(0, 0), ri.PreviewBounds.Size);            
            r.Inflate(-this.Painter.ElementsPainter.RowPreview.GetPreviewTextHorizontalPadding(this), -this.Painter.ElementsPainter.RowPreview.GetPreviewTextVerticalPadding(this));            
            return r;
        }
        public override int CalcRowPreviewHeight(int rowHandle)
        {
            RepositoryItem item = View.PreviewRowEdit;
            if (item == null)
                return base.CalcRowPreviewHeight(rowHandle);
            else
                return CalcRowPreviewEditorHeight(rowHandle, item);
        }
        protected virtual int CalcRowPreviewEditorHeight(int rowHandle, RepositoryItem item)
        {
            if (!View.OptionsView.ShowPreview || View.IsGroupRow(rowHandle) || View.IsFilterRow(rowHandle)) return 0;
            int res = (View.OptionsView.ShowPreviewRowLines != DevExpress.Utils.DefaultBoolean.False ? 1 : 0);
            int eventHeight = View.RaiseMeasurePreviewHeightAccessMetod(rowHandle);
            if (eventHeight != -1) return eventHeight == 0 ? 0 : res + eventHeight;
            Graphics g = GInfo.AddGraphics(null);
            try
            {
                IHeightAdaptable ha = fRowPreviewViewInfo as IHeightAdaptable;
                if (ha != null)
                {
                    fRowPreviewViewInfo.EditValue = View.GetRowPreviewValue(rowHandle);
                    res = ha.CalcHeight(GInfo.Cache, this.CalcRowPreviewWidth(rowHandle) - this.Painter.ElementsPainter.RowPreview.GetPreviewIndent(this) - this.Painter.ElementsPainter.RowPreview.GetPreviewTextHorizontalPadding(this));
                }
                res = Math.Max(fRowPreviewViewInfo.CalcMinHeight(g), res);
            }
            finally
            {
                GInfo.ReleaseGraphics();
            }
            res += this.Painter.ElementsPainter.RowPreview.GetPreviewTextVerticalPadding(this) * 2;
            return res;
        }
        protected override void CalcRowHitInfo(Point pt, GridRowInfo ri, GridHitInfo hi)
        {
            base.CalcRowHitInfo(pt, ri, hi);
        }
    }
    public class CustomGridHandler : BandedGridHandler
    {
        public CustomGridHandler(AdvBandedGridView gridView) : base(gridView) { }
        protected override GridRowNavigator CreateRowNavigator() { return new CustomGridRegularRowNavigator(this); }
    }
    public class CustomGridRegularRowNavigator : GridRegularRowNavigator
    {
        public CustomGridRegularRowNavigator(GridHandler handler) : base(handler) { }
        protected new CustomGridView View { get { return base.View as CustomGridView; } }
        public override bool OnMouseDown(GridHitInfo hitInfo, DevExpress.Utils.DXMouseEventArgs e)
        {
           
            bool res = base.OnMouseDown(hitInfo, e);
            if (hitInfo.HitTest == GridHitTest.RowPreview)
            {
                View.FocusedColumn = null;
                View.ActivatePreviewEditor();
                View.IsRowPreviewSelected = true;
            }
            else
                View.IsRowPreviewSelected = false;
            return res;
        }
    }
    public class CustomGridInfoRegistrator : AdvBandedGridInfoRegistrator
    {
        public CustomGridInfoRegistrator() : base() { }
     
        public override BaseViewPainter CreatePainter(BaseView view) { return new CustomGridPainter(view as AdvBandedGridView); }
        public override DevExpress.XtraGrid.Views.Base.ViewInfo.BaseViewInfo CreateViewInfo(BaseView view) { return new CustomGridViewInfo(view as AdvBandedGridView); }
        public override DevExpress.XtraGrid.Views.Base.Handler.BaseViewHandler CreateHandler(BaseView view) { return new CustomGridHandler(view as AdvBandedGridView); }
        public override string ViewName { get { return "CustomGridView"; } }
        public override BaseView CreateView(GridControl grid)
        {
            CustomGridView view = new CustomGridView();
            view.SetGridControlAccessMetod(grid);
            return view;
        }
    }
}
