using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DBNeon.Helpers;
using DBNeon.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBNeon.ViewModels
{
    public class VMBlocks : ViewModelBase
    {
        private Block selectedBlock;                        // Выбранный блок
        private string selectionValue;                      // Выбранная выборка
        private object selectCondition;                     // Выбранное условие выборки
        private ObservableCollection<Block> resultSearch;   // Переменная результат поиска
        private bool isMainBlocks;                          // Флаг отображения только главных блоков
        private int[] mainBlocks = new int[7];              // Массив идентификаторов типов главных блоков
        private Models.Type[] types;
        private NeonContext nc = new NeonContext();

        // Список блоков, полученных в результате поиска.
        public ObservableCollection<Block> ResultSearch
        {
            get => resultSearch;
            set
            {
                resultSearch = value;
                RaisePropertyChanged(nameof(ResultSearch));
            }
        }        

        // Критерий выборки блоков (например, номер блока, название местоположения или поставки).
        public object SelectCondition
        {
            get => selectCondition;
            set
            {
                selectCondition = value;
                if (value != null)
                {
                    switch (SelectionValue)
                    {
                        case "номеру":
                            ResultSearch = new ObservableCollection<Block>() { Blocks.Single(v => v.Number == (value as Block).Number) };
                            break;
                        case "типу":
                            ResultSearch = new ObservableCollection<Block>(Blocks.Where(b => b.TypeId == (value as Models.Type).Id).ToList());
                            break;
                        case "месту":
                            ResultSearch = new ObservableCollection<Block>(Blocks.Where(b => b.PlaceId == (value as Location).Id).ToList());
                            if (!IsMainBlocks)
                                ResultSearch = new ObservableCollection<Block>(ResultSearch.Where(x => mainBlocks.Contains((int)x.TypeId)));
                            break;
                        case "владельцу":
                            ResultSearch = new ObservableCollection<Block>(Blocks.Where(b => b.OwnerId == (value as Location).Id).ToList());
                            break;
                        case "поставке":
                            ResultSearch = new ObservableCollection<Block>(Blocks.Where(b => b.ProcurementId == (value as Procurement).Id).ToList());
                            break;                        
                    }
                }               
                RaisePropertyChanged(nameof(SelectCondition));
            }
        }

        // Название выборок.
        public string[] Selection { get; set; } = { "номеру", "типу", "месту", "владельцу", "поставке" };

        // Выбранная выборка.
        public string SelectionValue
        {
            get => selectionValue;
            set
            {
                selectionValue = value;
                RaisePropertyChanged(nameof(SelectionValue));
            }
        }

        // Коллекция всех блоков.
        public ObservableCollection<Block> Blocks { get; set; }

        // Выбранный блок из коллекции. 
        public Block SelectedBlock
        {
            get => selectedBlock;
            set
            {
                selectedBlock = value;
                EditCommand.RaiseCanExecuteChanged();
                DelCommand.RaiseCanExecuteChanged();
                //RaisePropertyChanged(nameof(SelectedBlock));
            }
        }

        // Типы блоков.
        public Models.Type[] Types
        {
            get => types;
            set
            {
                types = value;

                var names = from t in value select t.Name;
                List<string> mains = new List<string>() { "АВВС", "ФОЧ", "Крейт", "БПСМ", "ВПЧ", "ППК", "ФП4К" };
                int i = 0;
                foreach (var tp in names)
                {
                    if (mains.Contains(tp))
                    {
                        mainBlocks[i] = value.Single(a => a.Name == tp).Id;
                        mains.Remove(tp);
                        i++;
                    }
                }
            }
        }

        public Location[] Places { get; set; }                      // Массив всех объектов
        public Procurement[] Procurements { get; set; }             // Массив поставок
        public ObservableCollection<Moving> Movings { get; set; }   // Коллекция перемещений

        public RelayCommand AddCommand { get; private set; }    // Команда добавления
        public RelayCommand DelCommand { get; private set; }    // Команда удаления блока
        public RelayCommand EditCommand { get; private set; }   // Команда редактирования
        public RelayCommand ExcelCommand { get; private set; }  // Команда отправки в Excel

        // Отображение только основных типов блоков.
        public bool IsMainBlocks
        {
            get => isMainBlocks;
            set
            {
                isMainBlocks = value;
                ResultSearch = new ObservableCollection<Block>(Blocks.Where(b => b.PlaceId == (SelectCondition as Location).Id).ToList());
                if (!isMainBlocks)
                    ResultSearch = new ObservableCollection<Block>(ResultSearch.Where(x => mainBlocks.Contains((int)x.TypeId)));
                RaisePropertyChanged(nameof(IsMainBlocks));
            }
        }
       
        // Конструктор
        public VMBlocks()
        {
            // Прогрузка всех коллекций из БД.
            nc.Blocks.Load();
            Blocks = nc.Blocks.Local;
            nc.Movings.Load();
            Movings = nc.Movings.Local;
            nc.Types.Load();
            Types = nc.Types.ToArray();
            nc.Locations.Load();
            Places = nc.Locations.ToArray();
            nc.Procurements.Load();
            Procurements = nc.Procurements.ToArray();

            ExcelCommand = new RelayCommand(() =>
            {
                if (SelectCondition == null)
                    CollectionToExcel(Blocks, "Список всех блоков");
                else
                {
                    string textForHeader = "";
                    switch (SelectionValue)
                    {
                        case "номеру":
                            textForHeader = $"Описание блока с №{(SelectCondition as Block).Number}";                             
                            break;
                        case "типу":
                            textForHeader = $"Список всех {(SelectCondition as Models.Type).Name}";                            
                            break;
                        case "месту":
                            textForHeader = $"Список блоков на объекте \"{(SelectCondition as Location).Name}\"";
                            break;
                        case "владельцу":
                            textForHeader = $"Список блоков, принадлежащих месту \"{(SelectCondition as Location).Name}\"";
                            break;
                        case "поставке":
                            textForHeader = $"Список блоков из поставки \"{(SelectCondition as Procurement).Name}\"";
                            break;
                    }
                    CollectionToExcel(ResultSearch, textForHeader);
                }                    
            });

            // Команда добавления блока.
            AddCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessageAction("ShowWinBlocksAdd", SendMessForAddBlock)));

            // Команда удаления блока. Передаётся сообщение в окно с подтверждением удаления.
            DelCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessageAction<MessageBoxResult>($"Точно удалить блок с номером {SelectedBlock.Number}?", DeleteBlock)), 
                () => { return SelectedBlock != null; });
            
            // Команда изменения параметров блока. 
            // Не передаю ссылку на блок, чтобы изменения не вносились сразу из окна изменения.
            EditCommand = new RelayCommand(() => 
                Messenger.Default.Send(new NotificationMessageAction("ShowWinBlocksEdit", SendMessForChangeBlock)), () => { return SelectedBlock != null; });
            
            // Принимаю команду об изменении/добавлении блока.
            Messenger.Default.Register<MessAddNewBlock>(this, ReceiveAddEditBlock);
        }

        // Метод, выполняющийся при нажатии на кнопку "Удалить".
        public void DeleteBlock(MessageBoxResult result)
        {
            if (result == MessageBoxResult.OK)
            {
                var movs = Movings.Where(x => x.Number == SelectedBlock.Number).ToArray();
                for (int i = 0; i < movs.Length; i++)
                    nc.Movings.Remove(movs.ElementAt(i));
                nc.Blocks.Remove(SelectedBlock);
                nc.SaveChanges();

                Messenger.Default.Send(new NotificationMessage("Обновить последнее перемещение"));
            }
        }

        // Метод, выполняющийся при получении сообщения о подтверждении добавления/изменения.
        public void ReceiveAddEditBlock(MessAddNewBlock parameters)
        {
            if (parameters.IsEdit)
            {
                SelectedBlock.Number = parameters.NewBlock.Number;
                SelectedBlock.TypeId = parameters.NewBlock.TypeId;
                SelectedBlock.PlaceId = parameters.NewBlock.PlaceId;
                SelectedBlock.Date = parameters.NewBlock.Date;
                SelectedBlock.OwnerId = parameters.NewBlock.OwnerId;
                SelectedBlock.ProcurementId = parameters.NewBlock.ProcurementId;
                SelectedBlock.Decommissioned = parameters.NewBlock.Decommissioned;
                SelectedBlock.Description = parameters.NewBlock.Description;
                nc.Entry(SelectedBlock).State = EntityState.Modified;
                nc.SaveChanges();
            }
            else
            {
                try
                {
                    nc.Blocks.Add(parameters.NewBlock);
                    nc.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка добавления в базу", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Метод, вызываемый при нажатии на кнопку "Добавить". 
        public void SendMessForAddBlock()
        {
            Messenger.Default.Send(new MessForChangeBlock()
            {
                IsEdit = false,                                                 // Флаг редактирования блока.
                TypeId = Types.SingleOrDefault(r => r.Name == "АВВС").Id,       // Тип устанавливаю принудительно, чтоб не было null.
                PlaceId = Places.SingleOrDefault(y => y.Name == "к.359").Id,    // Устанавливаю местоположению по умолчанию.
                AllBlockNumbers = Blocks.Select(y => y.Number).ToList(),        // Передаю список всех названий блоков.
                TypesArray = Types,                                             // Массив типов. 
                LocationsArray = Places,                                        // Массив названий объектов.
                ProcurementsArray = Procurements                                // Массив поставок.
            });
        }

        // Метод, вызываемый при нажатии на кнопку "Изменить".
        public void SendMessForChangeBlock()
        {
            Messenger.Default.Send(new MessForChangeBlock()
            {
                IsEdit = true,
                Number = SelectedBlock.Number,
                TypeId = SelectedBlock.TypeId,
                PlaceId = SelectedBlock.PlaceId,
                Date = SelectedBlock.Date,
                OwnerId = SelectedBlock.OwnerId,
                ProcurementId = SelectedBlock.ProcurementId,
                Decommissioned = SelectedBlock.Decommissioned == 1,
                Description = SelectedBlock.Description,
                AllBlockNumbers = Blocks.Where(b => b.Number != SelectedBlock.Number).Select(y => y.Number).ToList(),
                TypesArray = Types,
                LocationsArray = Places,
                ProcurementsArray = Procurements
            });
        }

        // Отправка таблицы в Excel.
        public void CollectionToExcel(ObservableCollection<Block> collection, string headerText)
        {
            Excel.Application objExcel = new Excel.Application();
            Excel.Workbook workbook = objExcel.Workbooks.Add();
            Excel.Worksheet sheet = workbook.ActiveSheet;
            Excel.Range rangeForPaint = sheet.Range[sheet.Cells[2, 1],
                sheet.Cells[collection.Count + 2, 7]];

            Excel.Range rowForDescription = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 7]];
            rowForDescription.Merge(System.Type.Missing);
            rowForDescription.EntireColumn.HorizontalAlignment = Excel.Constants.xlCenter;
            rowForDescription.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rowForDescription.EntireRow.Font.Bold = true;

            Excel.Range headerRows = sheet.Range["A2", System.Type.Missing];
            headerRows.EntireRow.Font.Bold = true;

            sheet.Cells[1, 1] = headerText;

            sheet.Cells[2, 1] = "Номер";
            sheet.Cells[2, 2] = "Тип";
            sheet.Cells[2, 3] = "Местоположение";
            sheet.Cells[2, 4] = "Дата";
            sheet.Cells[2, 5] = "Владелец";
            sheet.Cells[2, 6] = "Поставка";
            sheet.Cells[2, 7] = "Списан";

            for (int i = 0; i < collection.Count; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j == 0)
                        rangeForPaint.Cells[i + 2, j + 1] = collection.ElementAt(i).Number;
                    else if (j == 1)
                        rangeForPaint.Cells[i + 2, j + 1] = Types.Single(w => w.Id == collection.ElementAt(i).TypeId).Name;
                    else if (j == 2)
                        rangeForPaint.Cells[i + 2, j + 1] = Places.Single(q => q.Id == collection.ElementAt(i).PlaceId).Name;
                    else if (j == 3)
                        rangeForPaint.Cells[i + 2, j + 1] = collection.ElementAt(i).Date;
                    else if (j == 4)
                        rangeForPaint.Cells[i + 2, j + 1] =
                            Places.SingleOrDefault(q => q.Id == collection.ElementAt(i).OwnerId) != null
                                ? Places.Single(q => q.Id == collection.ElementAt(i).OwnerId).Name
                                : "";
                    else if (j == 5)
                        rangeForPaint.Cells[i + 2, j + 1] =
                            Procurements.SingleOrDefault(y => y.Id == collection.ElementAt(i).ProcurementId) != null
                                ? Procurements.Single(y => y.Id == collection.ElementAt(i).ProcurementId).Name
                                : "";
                    else
                        rangeForPaint.Cells[i + 2, j + 1] = collection.ElementAt(i).Decommissioned == 1 ? "да" : "нет";
                }
            }

            rangeForPaint.Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rangeForPaint.EntireColumn.AutoFit();

            objExcel.Visible = true;
        }
    }
}
