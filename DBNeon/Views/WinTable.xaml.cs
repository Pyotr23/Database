using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBNeon.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Views
{
    /// <summary>
    /// Логика взаимодействия для WinTable.xaml
    /// </summary>
    public partial class WinTable : Window
    {
        private string[] rowHeaders;
        private string[] mainBlocks = {"АВВС", "ФОЧ", "Крейт", "БПСМ", "ВПЧ", "ППК", "ФП4К"};

        public WinTable()
        {
            

            InitializeComponent();
            Messenger.Default.Register<MessSendHeaders>(this, data => rowHeaders = data.RowHeaders);
            Messenger.Default.Register<MessHideColumns>(this, data => HideOrNotColumns(data.Hide));

            Closing += (s, e) => Cleanup();
        }

        private void HideOrNotColumns(bool hide)
        {
            if (hide)
            {
                for (int i = 0; i < dtgTable.Columns.Count; i++)
                {
                    if (mainBlocks.Contains(dtgTable.Columns[i].Header))
                        dtgTable.Columns[i].Visibility = Visibility.Visible;
                    else
                        dtgTable.Columns[i].Visibility = Visibility.Hidden;
                }
            }
            else
            {
                for (int i = 0; i < dtgTable.Columns.Count; i++)
                    dtgTable.Columns[i].Visibility = Visibility.Visible;
            }
            
        }

        private void DtgTable_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = rowHeaders[e.Row.GetIndex()];
        }

        private void Cleanup()
        {
            Messenger.Default.Unregister<MessSendHeaders>(this);
            Messenger.Default.Unregister<MessHideColumns>(this);
        }

        private void DtgTable_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            (sender as DataGrid).SelectedCells.Clear();
        }
    }
}
