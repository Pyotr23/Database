using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DBNeon.Helpers;
using DBNeon.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBNeon.ViewModels
{
    public class VMTable : ViewModelBase
    {
        // Поля 
        //private Visibility isVisible;
        private bool isChecked = false;

        private DataView table;

        // Свойства
        public Block[] Blocks { get; set; }
        public Models.Type[] Types { get; set; }
        public string[] TypesNames { get; set; }
        public Location[] Locations { get; set; }
        public string[] LocsNames { get; set; }
        public string[][] CountBlocks { get; set; }

        public DataView Table
        {
            get => table;
            set
            {
                table = value;
                RaisePropertyChanged(nameof(Table));
            }
        }
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                Messenger.Default.Send(new MessHideColumns() { Hide = value });
                RaisePropertyChanged(nameof(IsChecked));
            }
        }

        public RelayCommand CommSendToExcel { get; set; }

        // Конструктор
        public VMTable()
        {
            NeonContext nc = new NeonContext();
            nc.Blocks.Load();
            Blocks = nc.Blocks.Local.ToArray();

            nc.Types.Load();
            Types = nc.Types.Local.OrderBy(a => a.Name).ToArray();
            TypesNames = Types.Select(n => n.Name).ToArray();

            nc.Locations.Load();
            Locations = nc.Locations.Local.OrderBy(a => a.Name).ToArray();
            LocsNames = Locations.Select(n => n.Name).ToArray();
            Messenger.Default.Send(new MessSendHeaders() { RowHeaders = LocsNames });

            CountBlocks = new string[LocsNames.Count()][];

            for (int i = 0; i < LocsNames.Count(); i++)
            {
                string[] tmp = new string[TypesNames.Count()];
                for (int j = 0; j < TypesNames.Count(); j++)
                {
                    tmp[j] = Blocks.Where(b => b.PlaceId == Locations.Single(l => l.Name == LocsNames[i]).Id).
                        Count(b => b.TypeId == Types.Single(t => t.Name == TypesNames[j]).Id).ToString();
                }
                CountBlocks[i] = tmp;
            }

            Table = CreateDataView(TypesNames, LocsNames, CountBlocks);    
            
            CommSendToExcel = new RelayCommand(() =>
            {
                DataTable tableToExcel = Table.ToTable();
                if (IsChecked)
                {
                    string[] mainBlocks = { "АВВС", "ФОЧ", "Крейт", "БПСМ", "ВПЧ", "ППК", "ФП4К" };
                    for (int i = 0; i < tableToExcel.Columns.Count; i++)
                    {
                        if (!mainBlocks.Contains(tableToExcel.Columns[i].ColumnName))
                            tableToExcel.Columns.RemoveAt(i);
                    }
                }
                Excel.Application objExcel = new Excel.Application();
                Excel.Workbook workbook = objExcel.Workbooks.Add();
                Excel.Worksheet sheet = workbook.ActiveSheet;
                Excel.Range range = sheet.Range[sheet.Cells[1, 1],
                    sheet.Cells[tableToExcel.Rows.Count + 1, tableToExcel.Columns.Count + 1]];
                for (int i = 0; i < tableToExcel.Columns.Count; i++)
                    range.Cells[1, 2 + i] = tableToExcel.Columns[i].Caption;
                for (int i = 0; i < tableToExcel.Rows.Count; i++)
                    range.Cells[2 + i, 1] = LocsNames[i];

                for (int i = 0; i < tableToExcel.Rows.Count; ++i)
                for (int j = 0; j < tableToExcel.Columns.Count; ++j)
                    range.Cells[2 + i, 2 + j] = tableToExcel.Rows[i][j].ToString();

                sheet.Cells.EntireColumn.ColumnWidth = 5.5;
                sheet.Cells.EntireColumn.HorizontalAlignment = Excel.Constants.xlCenter;
                range.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                Excel.Range headerRows = sheet.Range["A1", System.Type.Missing];
                headerRows.EntireColumn.AutoFit();
                headerRows.EntireColumn.HorizontalAlignment = Excel.Constants.xlLeft;
                headerRows.EntireColumn.Font.Bold = true;

                Excel.Range headerColumns = sheet.Cells[1, 1];
                headerColumns.EntireRow.Font.Bold = true;

                sheet.Cells.EntireRow.AutoFit();
                objExcel.Visible = true;
            });
        }

        // Методы
        public DataView CreateDataView(string[] columns, string[] rows, string[][] data)
        {
            DataTable myDataTable = new DataTable();

            foreach (string headerName in columns)
                myDataTable.Columns.Add(headerName);

            for (int i = 0; i < rows.Length; i++)
                myDataTable.Rows.Add(data[i]);

            return myDataTable.DefaultView;
        }
    }
}
