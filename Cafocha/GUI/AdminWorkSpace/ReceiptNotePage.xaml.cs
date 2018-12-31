using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper.Report;
using Cafocha.Repository.DAL;
using POS.AdminWorkSpace;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for ReceiptNotePage.xaml
    /// </summary>
    public partial class ReceiptNotePage : Page
    {
        private BusinessModuleLocator _businessModuleLocator;
        List<Ingredient> _ingrelist;
        List<ReceiptNote> _relist;
        List<ReceiptNoteDetail> _rnlist;
        public ReceiptNotePage(BusinessModuleLocator businessModuleLocator, AdminRe admin)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            _relist = _businessModuleLocator.ReceiptNoteModule.getAllReceiveNotes().ToList();
            _relist = _relist.Where(x => x.Employee.Manager.Equals(admin.AdId)).ToList();
            lvReceptNote.ItemsSource = _relist;
            _rnlist = _businessModuleLocator.ReceiptNoteModule.getAllReceiveNoteDetails().ToList();
            List<ReceiptNoteDetail> _rnTempList = new List<ReceiptNoteDetail>();
            foreach (var receiptdetails in _rnlist)
            {
                bool found = false;
                foreach (var receiptnote in _relist)
                {
                    if (receiptdetails.RnId.Equals(receiptnote.RnId))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    _rnTempList.Add(receiptdetails);
            }
            _rnlist = _rnTempList;
            lvReceiptNoteDetail.ItemsSource = _rnlist;

            this.Loaded += ReceiptNotePage_Loaded;
        }

        private void ReceiptNotePage_Loaded(object sender, RoutedEventArgs e)
        {
            _ingrelist = _businessModuleLocator.IngredientModule.getAllIngredients().ToList();
            initData();
        }

        private void initData()
        {
            isRaiseEvent = false;
            List<dynamic> ingl = new List<dynamic>();
            ingl.Add(new { Id = "--", Name = "--" });
            foreach (var p in _ingrelist)
            {
                ingl.Add(new { Id = p.IgdId, Name = p.Name });
            }

            cboIngre.ItemsSource = ingl;
            cboIngre.SelectedValuePath = "Id";
            cboIngre.DisplayMemberPath = "Name";
            cboIngre.SelectedValue = "--";
            isRaiseEvent = true;
        }

        private void lvReceptNote_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReceiptNote rn = lvReceptNote.SelectedItem as ReceiptNote;
            if (rn != null)
            {
                lvReceiptNoteDetail.ItemsSource = _businessModuleLocator.ReceiptNoteModule.getReceiptNoteDetails(rn.RnId);
            }
            else
            {
                lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
            }
        }

        List<ReceiptNoteDetail> filterrn = new List<ReceiptNoteDetail>();
        List<ReceiptNote> filterre = new List<ReceiptNote>();
        bool isRaiseEvent = true;
        private void cboIngre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterre = new List<ReceiptNote>();
            lvReceptNote.UnselectAll();
            lvReceiptNoteDetail.UnselectAll();
            if (isRaiseEvent)
            {
                ComboBox cboi = sender as ComboBox;
                string ingid = cboi.SelectedValue.ToString();
                if (!ingid.Equals("--"))
                {
                    filterrn = _rnlist.Where(x => x.IgdId.Equals(ingid)).ToList();
                    var odd = filterrn.GroupBy(x => x.RnId).Select(y => y.ToList()).ToList();

                    foreach (var i in odd)
                    {
                        foreach (var j in i)
                        {
                            filterre.Add(_relist.Where(x => x.RnId.Equals(j.RnId)).FirstOrDefault());
                            break;
                        }
                    }

                    if (filterre.Count != 0 && pickOrderDate.SelectedDate == null)
                    {
                        lvReceptNote.ItemsSource = filterre;
                        lvReceptNote.Items.Refresh();
                        lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                        lvReceiptNoteDetail.Items.Refresh();
                    }
                    else if (filterre.Count != 0 && pickOrderDate.SelectedDate != null)
                    {
                        lvReceptNote.ItemsSource = filterre.Where(x => x.Inday.ToShortDateString().Equals(((DateTime)pickOrderDate.SelectedDate).ToShortDateString())).ToList();
                        lvReceptNote.Items.Refresh();
                        lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                        lvReceiptNoteDetail.Items.Refresh();
                    }
                    else
                    {
                        lvReceptNote.ItemsSource = new List<ReceiptNote>();
                        lvReceptNote.Items.Refresh();
                        lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                        lvReceiptNoteDetail.Items.Refresh();
                    }
                }
                else
                {
                    if (pickOrderDate.SelectedDate == null)
                    {
                        lvReceptNote.ItemsSource = _relist;
                        lvReceptNote.Items.Refresh();
                        lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                        lvReceiptNoteDetail.Items.Refresh();
                    }
                    else
                    {
                        lvReceptNote.ItemsSource = _relist.Where(x => x.Inday.ToShortDateString().Equals(((DateTime)pickOrderDate.SelectedDate).ToShortDateString())).ToList();
                        lvReceptNote.Items.Refresh();
                        lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                        lvReceiptNoteDetail.Items.Refresh();
                    }
                }
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void pickOrderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker pick = sender as DatePicker;
            if (pick.SelectedDate == null)
            {
                return;
            }

            if (cboIngre.SelectedValue.Equals("--"))
            {
                lvReceptNote.ItemsSource = _relist.Where(x => x.Inday.ToShortDateString().Equals(((DateTime)pick.SelectedDate).ToShortDateString()));
                lvReceptNote.Items.Refresh();
                lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                lvReceiptNoteDetail.Items.Refresh();
            }
            else
            {
                if (filterre.Count != 0)
                {
                    lvReceptNote.ItemsSource = filterre.Where(x => x.Inday.ToShortDateString().Equals(((DateTime)pick.SelectedDate).ToShortDateString()));
                    lvReceptNote.Items.Refresh();
                }
                else
                {
                    lvReceptNote.ItemsSource = new List<ReceiptNote>();
                    lvReceptNote.Items.Refresh();
                }

                lvReceiptNoteDetail.ItemsSource = new List<ReceiptNoteDetail>();
                lvReceiptNoteDetail.Items.Refresh();
            }
        }

        private void BtnOverViewReport_OnClick(object sender, RoutedEventArgs e)
        {
            var optionDialog = new ReportOptionDialog(new ReceiptNoteReport(), _businessModuleLocator);
            optionDialog.Show();
        }

    }
}
