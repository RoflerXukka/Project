﻿using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserInterface
{
    public partial class Users : Form
    {
        List<Thread> threads = new List<Thread>();
        private Search_form sf;
        private Books friend;
        private int countOfTrueRows = 0;
        private bool Lock_Flag { get; set; }
        private List<List<DataGridViewRow>> Books;
        private List<List<string>> DatesOfAddBooks;
        public DataGridView DataUserGrid => DataUsers;
        public delegate Task MarkElement(int indx);
        public event MarkElement mark; // событие отмечания, пометки строки таблицы
        public DataGridView DataCurrentUser => DataUserInf;
        private DataGridViewRow item;
        public Users()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            if (!File.Exists("BD.xml"))
            {
                MessageBox.Show("Не удалось открыть файл", "Ошибка");
                Application.Exit();
            }
            Books = new List<List<DataGridViewRow>>();
            DatesOfAddBooks = new List<List<string>>();
            InitializeComponent();
            Lock_Flag = false;
            Delete.Enabled = false;
            MoveObj.BackColor = System.Drawing.Color.Transparent;
            CLOSE.FlatAppearance.BorderSize = 0;
            MINIMASE.FlatAppearance.BorderSize = 0;
            friend = new Books(this);
            // впихнуть синхронизацию
            //Thread thread = new Thread(friend.LoadMainData);
            //thread.Start(wait);
            //thread.Join();
            friend.LoadMainData(1);
            LoadMainData();
            //wait.WaitOne();
            friend.Show();
            mark += MarkThisElement;

        } // происходит при инициализации формы
        #region Анимация
        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                cp.ClassStyle &= CS_DROPSHADOW;
                return cp;
            }
        } // для тени формы
        private void MoveObj_MouseDown(object sender, MouseEventArgs e)
        {
            MoveObj.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }
        private void MINIMASE_MouseMove(object sender, MouseEventArgs e) => MINIMASE.BackColor = Color.Silver;
        private void MINIMASE_MouseLeave(object sender, EventArgs e) => MINIMASE.BackColor = Color.Gainsboro;
        private void CLOSE_MouseMove(object sender, MouseEventArgs e)
        {
            CLOSE.BackColor = Color.Crimson;
            CLOSE.ForeColor = Color.White;
        }  // ЭТО ВСЁ ДЛЯ КРАСИВОЙ АНИМАЦИИ
        private void CLOSE_MouseLeave(object sender, EventArgs e)
        {
            CLOSE.BackColor = Color.Gainsboro;
            CLOSE.ForeColor = Color.Black;
        } // И ЭТО
        private void MainForm_Deactivate(object sender, EventArgs e) => MoveObj.Focus();
        private void MINIMASE_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;
        private void CLOSE_Click(object sender, EventArgs e) => Application.Exit(); // ЭТО ТОЖЕ
        #endregion
        #region Служебные методы, облегчающие разработку
        public void UpdateDataUserInf()
        {
            if(DataUsers.SelectedRows.Count != 0)
                DataUsers_SelectionChanged(1, new EventArgs());
        }
        public void AddBookDrDr(DataGridViewRow row)
        {
            Books[DataUsers.CurrentRow.Index].Add(row);
            DatesOfAddBooks[DataUsers.CurrentRow.Index].Add(DateTime.Today.ToShortDateString());
            int n = DataUserInf.Rows.Add();
            DataUserInf.Rows[n].Cells[0].Value = Books[DataUsers.CurrentRow.Index][Books[DataUsers.CurrentRow.Index].Count - 1].Cells[0].Value.ToString();
            DataUserInf.Rows[n].Cells[1].Value = Books[DataUsers.CurrentRow.Index][Books[DataUsers.CurrentRow.Index].Count - 1].Cells[1].Value.ToString();
            DataUserInf.Rows[n].Cells[2].Value = DatesOfAddBooks[DataUsers.CurrentRow.Index][DatesOfAddBooks[DataUsers.CurrentRow.Index].Count - 1];
        }
        public void RemoveingUser(int IndexBook, int IndexUser)
        {
            for (int i = 0; i < Books[IndexUser].Count; i++)
            {
                if (Books[IndexUser][i].Index == IndexBook)
                {
                    Books[IndexUser].RemoveAt(i);
                    break;
                }
            }

        }
        public void RemoveingBook(List<DataGridViewRow> list, int index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < Books[list[i].Index].Count; j++)
                {
                    if (Books[list[i].Index][j].Index == index)
                    {
                        Books[list[i].Index].RemoveAt(j);
                        break;
                    }
                }
            }

        }
        public void LoadMainData()
        {
            DataSet dataalll = new DataSet();
            dataalll.ReadXml("BD.xml");
            try
            {
                int n;
                foreach (DataRow row in dataalll.Tables["Users"].Rows)
                {
                    n = DataUsers.Rows.Add(); // добавляем новую сроку в dataGridView1
                    DataUsers.Rows[n].Cells[0].Value = row["UserName"]; // заносим в первый столбец созданной строки данные из первого столбца таблицы ds.
                    DataUsers.Rows[n].Cells[1].Value = row["Adress"]; // то же самое со вторым столбцом
                }
            }
            catch
            {
                return;
            }
        }
        public void AddToDATA(string first, string second)
        {
            DataUsers.Rows.Add();
            DataUsers.Rows[DataUsers.Rows.Count - 1].Cells[0].Value = first;
            DataUsers.Rows[DataUsers.Rows.Count - 1].Cells[1].Value = second;
            DataUsers.Rows[DataUsers.Rows.Count - 1].Cells[2].Value = 0;
            Books.Add(new List<DataGridViewRow>());
            DatesOfAddBooks.Add(new List<string>());
        } // это метод добавления пользователя или книги
        public (string User, string Adress) GetDataUser(int index) => (DataUsers.Rows[index].Cells[0].Value.ToString(), DataUsers.Rows[index].Cells[1].Value.ToString());
        private (string, string) GetDataRow(DataGridViewRow row) => (row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());

        #endregion
        private async Task MarkThisElement(int indx) => Task.Run(() => { DataUsers.Rows[indx].Visible = false; });
        private async void DataUsers_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    case MouseButtons.Left when (DataUsers.CurrentRow.Selected):
                        DataUserInf.Rows.Clear();
                        for (int i = 0; i < Books[DataUsers.CurrentRow.Index].Count; i++)
                        {
                            int n = DataUserInf.Rows.Add();
                            (DataUserInf.Rows[n].Cells[0].Value, DataUserInf.Rows[n].Cells[1].Value) = GetDataRow(Books[DataUsers.CurrentRow.Index][i]);
                            DataUserInf.Rows[n].Cells[2].Value = DatesOfAddBooks[DataUsers.CurrentRow.Index][i];
                        }
                        DataUserInf.ClearSelection();
                        Delete.Enabled = true;
                        Delete.Text = "Удалить пользователя";
                        break;
                    case MouseButtons.Middle:
                        await Helper.UnHideAllRowsAsync(DataUsers, this);
                        DataUsers.ClearSelection();
                        DataUserInf.Rows.Clear();
                        Delete.Text = "Удалить...";
                        Delete.Enabled = false;
                        InfoLabelUserBookCount.Visible = InfoLabelUserBookCountValue.Visible = false;
                        break;
                }
            }
            catch { }
        } // обычный клик на таблицу с пользователями
        private void MainForm_Load(object sender, EventArgs e)
        {
            DataSet dataalll = new DataSet();
            dataalll.ReadXml("BD.xml");
            try
            {
                int i = 0;
                int n = 0;
                foreach (DataRow row in dataalll.Tables["Users"].Rows)
                {
                    DataUsers.Rows[n++].Cells[2].Value = row["NberReatedBooks"]; // то же самое с третьим столбцом
                    string[] indexes = row["IndexesOfCurrentBooks"].ToString().Split('.');
                    string[] dates = row["DatesOfAddBooks"].ToString().Split(';');
                    int j = 0;
                    foreach (string index in indexes)
                    {
                        Books.Add(new List<DataGridViewRow>());
                        DatesOfAddBooks.Add(new List<string>());
                        try
                        {
                            this.Books[i].Add(friend.DataBooksGrid.Rows[Convert.ToInt32(index)]);
                            DatesOfAddBooks[i].Add(dates[j++]);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    i++;
                }
                DataUsers.ClearSelection();
                MoveObj.Focus();
            }
            catch
            {
                return;
            }
            this.DataUsers.SelectionChanged += new System.EventHandler(this.DataUsers_SelectionChanged);
            CountOfUsersTotal.Text = DataUsers.RowCount.ToString();
        } // происходит при загрузке формы
        private void Delete_Click(object sender, EventArgs e)
        {
            if (Delete.Text == "Удалить пользователя")
            {
                DataUserInf.Rows.Clear();
                friend.RemoveingUser(Books[DataUsers.CurrentRow.Index], DataUsers.CurrentRow.Index);
                Books.RemoveAt(DataUsers.CurrentRow.Index);
                DatesOfAddBooks.RemoveAt(DataUsers.CurrentRow.Index);
                DataUsers.Rows.RemoveAt(DataUsers.CurrentRow.Index);
                if (DataUsers.Rows.Count == 0)
                {
                    Delete.Enabled = false;
                    Delete.Text = "Удалить...";
                }
                CountOfUsersTotal.Text = DataUsers.RowCount.ToString();
            }
            else
            {
                friend.RemoveingBook(DataUsers.CurrentRow.Index, Books[DataUsers.CurrentRow.Index][DataUserInf.CurrentRow.Index].Index);
                Books[DataUsers.CurrentRow.Index].RemoveAt(DataUserInf.CurrentRow.Index);
                DataUserInf.Rows.RemoveAt(DataUserInf.CurrentRow.Index);
                if (DataUserInf.RowCount == 0)
                {
                    Delete.Enabled = false;
                    Delete.Text = "Удалить...";
                }
                InfoLabelUserBookCountValue.Text = DataUserInf.RowCount.ToString();
            }
            friend.UpdateDataBooksInf();
        } // удаление выбранной книги / выбранного пользователя
        private void AddTooo_Click(object sender, EventArgs e)
        {
            AddPoly addpoly = new AddPoly(this);
            addpoly.ShowDialog();
        } // ОКНО ДОБАВЛЕНИЯ ДАННЫХ (пользователь или книга)
        private void DataUserInf_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        DataUserInf.ClearSelection();
                        Delete.Enabled = false;
                        Delete.Text = "Удалить...";
                        break;
                    case MouseButtons.Left when (DataUserInf.CurrentRow.Selected):
                        Delete.Enabled = true;
                        Delete.Text = "Убрать книгу";
                        break;
                }
            }
            catch (Exception) { }
        }
        private void DataUsers_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DataUserInf.Rows.Clear();
            friend.RemoveingUser(Books[e.Row.Index], e.Row.Index);
            Books.RemoveAt(e.Row.Index);
            DatesOfAddBooks.RemoveAt(e.Row.Index);
            friend.UpdateDataBooksInf();
            CountOfUsersTotal.Text = (DataUsers.RowCount - 1).ToString();
        }
        private void DataUserInf_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            friend.RemoveingBook(DataUsers.CurrentRow.Index, Books[DataUsers.CurrentRow.Index][e.Row.Index].Index);
            Books[DataUsers.CurrentRow.Index].RemoveAt(e.Row.Index);
            if (DataUserInf.Rows.Count == 1)
            {
                Delete.Enabled = false;
                Delete.Text = "Удалить...";
            }
            InfoLabelUserBookCountValue.Text = (DataUserInf.RowCount - 1).ToString();
            friend.UpdateDataBooksInf();
        }
        private void DataUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (DataUsers.RowCount == 0)
            {
                Delete.Enabled = false;
                Delete.Text = "Удалить...";
                return;
            }
            DataUserInf.Rows.Clear();
            for (int i = 0; i < Books[DataUsers.CurrentRow.Index].Count; i++)
            {
                int n = DataUserInf.Rows.Add();
                (DataUserInf.Rows[n].Cells[0].Value, DataUserInf.Rows[n].Cells[1].Value) = GetDataRow(Books[DataUsers.CurrentRow.Index][i]);
                DataUserInf.Rows[n].Cells[2].Value = DatesOfAddBooks[DataUsers.CurrentRow.Index][i];
            }
            DataUserInf.ClearSelection();
            InfoLabelUserBookCount.Visible = InfoLabelUserBookCountValue.Visible = true;
            InfoLabelUserBookCountValue.Text = DataUserInf.RowCount.ToString();
            Delete.Enabled = true;
            Delete.Text = "Удалить пользователя";
        }
        private void DataUsers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Right || DataUsers.RowCount == 0 || !DataUsers.CurrentRow.Selected || DataUserInf.RowCount == 0) return;
            DataUserInf.Rows[0].Selected = true;
            DataUserInf.Focus();
            Delete.Text = "Убрать книгу";
        }
        private void DataUserInf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left) return;
            DataUsers.Focus();
            Delete.Text = "Удалить пользователя";
            Delete.Enabled = true;
            DataUserInf.ClearSelection();
        }
        private void DataUsers_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex == -1) return;
            Lock_Flag = false;
            DataUsers.CurrentCell = DataUsers.Rows[e.RowIndex].Cells[0];///////////////////////////
            DataUsers.CurrentRow.Selected = true;
            Lock_Flag = true;
            DataUsers_SelectionChanged(1, new EventArgs());
            try { if (!friend.DataBooksGrid.CurrentRow.Selected) return; }
            catch { return; }
            friend.DataCurrentBook.AllowDrop = true;
            friend.DataCurrentBook.DoDragDrop(DataUsers.CurrentRow, DragDropEffects.All);
            friend.DataCurrentBook.AllowDrop = false;
        }
        private void DataUserInf_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            item = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
        }
        private void DataUserInf_DragDrop(object sender, DragEventArgs e)
        {
            for (int i = 0; i < DataUserInf.Rows.Count; i++)
                if (DataUserInf.Rows[i].Cells[0].Value.ToString() == item.Cells[0].Value.ToString() && DataUserInf.Rows[i].Cells[1].Value.ToString() == item.Cells[1].Value.ToString())
                {
                    MessageBox.Show("Данная книга уже имеется у выбранного Вами пользователя", "Ошибка добавления");
                    return;
                }
            int n = DataUserInf.Rows.Add();
            DataUserInf.Rows[n].Cells[0].Value = item.Cells[0].Value.ToString();
            DataUserInf.Rows[n].Cells[1].Value = item.Cells[1].Value.ToString();
            DataUserInf.Rows[n].Cells[2].Value = DateTime.Today.ToShortDateString();
            Books[DataUsers.CurrentRow.Index].Add(item);
            DatesOfAddBooks[DataUsers.CurrentRow.Index].Add(DataUserInf.Rows[n].Cells[2].Value.ToString());
            friend.AddUserDrDr(DataUsers.CurrentRow);
        }
        private void Users_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataSet dataalll = new DataSet();
            dataalll.ReadXml("BD.xml");
            try { dataalll.Tables.Remove("Users"); }
            catch { } // удаление старой таблицы
            DataTable dt = new DataTable();
            dt.TableName = "Users";
            dt.Columns.Add("UserName");
            dt.Columns.Add("Adress");
            dt.Columns.Add("NberReatedBooks");
            dt.Columns.Add("IndexesOfCurrentBooks");
            dt.Columns.Add("DatesOfAddBooks");
            dataalll.Tables.Add(dt);
            foreach (DataGridViewRow row in DataUsers.Rows)
            {
                DataRow data = dataalll.Tables["Users"].NewRow();
                data["UserName"] = row.Cells[0].Value.ToString();
                data["Adress"] = row.Cells[1].Value.ToString();
                data["NberReatedBooks"] = row.Cells[2].Value.ToString();
                string indexesanddate = "";
                for (int i = 0; i < Books[row.Index].Count; i++)
                    indexesanddate += Books[row.Index][i].Index.ToString() + '.';
                data["IndexesOfCurrentBooks"] = indexesanddate;
                indexesanddate = "";
                for (int i = 0; i < DatesOfAddBooks[row.Index].Count; i++)
                    indexesanddate += DatesOfAddBooks[row.Index][i] + ';';
                data["DatesOfAddBooks"] = indexesanddate;
                dataalll.Tables["Users"].Rows.Add(data);
            } // создание таблицы с данными 
            dataalll.WriteXml("BD.xml"); // сохранение данных в файл
        }
        private void Search_Click(object sender, EventArgs e)
        {
            sf = new Search_form(this);
            sf.ShowDialog();
        }
        public async void Searching(SearchData data) 
        {
            countOfTrueRows = 0;
            await Helper.UnHideAllRowsAsyncUnsafe(DataUsers);
            if (data.UseMultiThreading)
            {
                if (DataUsers.RowCount < Environment.ProcessorCount * 2)
                {
                    data.UseMultiThreading = false;
                    await ThreadSearch(data);
                    goto cont;
                }
                int first_index = 0, second_index, step;
                SearchData[] datas = new SearchData[Environment.ProcessorCount * 2];
                step = DataUsers.RowCount / datas.Length;
                second_index = step;
                for (int i = 0; i < Environment.ProcessorCount * 2; i++)
                {
                    datas[i] = new SearchData();
                    datas[i].first_index = first_index;
                    datas[i].second_index = second_index;
                    datas[i].str = data.str;
                    datas[i].numberOfColumn = data.numberOfColumn;
                    datas[i].RegistrON = data.RegistrON;
                    datas[i].UseWordWhole = data.UseWordWhole;
                    first_index += step;
                    second_index += step;
                }
                if (first_index < DataUsers.RowCount)
                {
                    data.first_index = first_index;
                    data.second_index = DataUsers.RowCount;
                    ThreadSearch(data);
                }
                for (int i = 0; i < datas.Length - 1; i++) ThreadSearch(datas[i]);
                await ThreadSearch(datas[datas.Length - 1]);
            }
            else
            {
                await ThreadSearch(data);
            }
            cont:
            DataUsers.BackgroundColor = Color.FromArgb(255, 255, 255);
            MessageBox.Show("Совпадений найдено: " + countOfTrueRows.ToString());
            int x = DataUsers.Size.Height;
            int y = DataUsers.Size.Width;
            DataUsers.Height = 0;
            DataUsers.Width = 0;
            DataUsers.Height = x;
            DataUsers.Width = y;
            sf.Close();
        }
        public void ShowResultOfSearch(int result)
        {

        }
        private Task ThreadSearch(SearchData inf) // заменить объект на кучу полей (да, херовенько будет, но ладно)
        {
            return Task.Factory.StartNew(() => {
                int flag = 0;
                string str = inf.str;
                int countOfResults = 0;
                string buf_str;
                if (inf.RegistrON)
                {
                    if (inf.UseWordWhole)
                    {
                        string[] str_words = str.Split(' ');
                        string[] buf_words;
                        if (inf.numberOfColumn == 5)
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    buf_words = DataUsers.Rows[i].Cells[j].Value.ToString().Split(' ');
                                    if (buf_words.Length == str_words.Length)
                                    {
                                        for (int indx = 0; indx < str_words.Length; indx++)
                                        {
                                            if (str_words[indx] != buf_words[indx])
                                            {
                                                goto lalala;
                                            }
                                        }
                                        flag++;
                                        j = 3;
                                        break;
                                    }
                                    if (buf_words.Length < str_words.Length)
                                    {
                                        continue;
                                    }
                                    for (int indx = 0; indx < buf_words.Length - str_words.Length + 1; indx++)
                                    {
                                        if (buf_words[indx] == str_words[0])
                                        {
                                            for (int I = 0; I < str_words.Length; I++)
                                            {
                                                if (str_words[I] != buf_words[indx + I])
                                                {
                                                    goto сontinue;
                                                }
                                            }
                                            flag++;
                                            j = 3;
                                        }
                                        break;
                                        сontinue:;
                                    }
                                    lalala:;
                                }
                                if (flag == 0)
                                {
                                    BeginInvoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    continue;
                                }
                                else
                                {
                                    countOfResults++;
                                    flag = 0;
                                }
                            }
                        }
                        else
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                buf_words = DataUsers.Rows[i].Cells[inf.numberOfColumn].Value.ToString().Split(' ');
                                if (buf_words.Length == str_words.Length)
                                {
                                    for (int indx = 0; indx < str_words.Length; indx++)
                                    {
                                        if (str_words[indx] != buf_words[indx])
                                        {
                                            Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                            goto lalala;
                                        }
                                    }
                                    countOfResults++;
                                    continue;
                                }
                                if (buf_words.Length < str_words.Length)
                                {
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    continue;
                                }

                                for (int indx = 0; indx < buf_words.Length - str_words.Length + 1; indx++)
                                {
                                    if (buf_words[indx] == str_words[0])
                                    {
                                        for (int I = 0; I < str_words.Length; I++)
                                        {
                                            if (str_words[I] != buf_words[indx + I])
                                            {
                                                goto сontinue;
                                            }
                                        }
                                        flag++;
                                        countOfResults++;
                                        goto Break;
                                    }
                                    сontinue:;
                                }
                                Break:
                                if (flag == 0)
                                {
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                }
                                flag = 0;
                                lalala:;
                            }

                        }
                    }
                    else
                    {
                        string bufferWord = null;
                        if (inf.numberOfColumn == 5)
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    bufferWord = DataUsers.Rows[i].Cells[j].Value.ToString();
                                    if (bufferWord.Length < str.Length) continue;
                                    if (!bufferWord.Contains(str)) continue;
                                    flag++;
                                    countOfResults++;
                                    break;
                                }
                                if(flag == 0)
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                flag = 0;
                            }
                        }
                        else
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                bufferWord = DataUsers.Rows[i].Cells[inf.numberOfColumn].Value.ToString();
                                if (bufferWord.Length < str.Length)
                                {
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    continue;
                                }
                                if (!bufferWord.Contains(str))
                                {
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    continue;
                                }
                                countOfResults++;
                            }

                        }
                    }
                }
                else
                {
                    string bufferWord = null;
                    str = str.ToLower(); // и так со всеми строками
                    if (inf.UseWordWhole)
                    {
                        string[] str_words = str.Split(' ');
                        string[] buf_words;
                        if (inf.numberOfColumn == 5)
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    buf_words = DataUsers.Rows[i].Cells[j].Value.ToString().ToLower().Split(' ');
                                    if (buf_words.Length == str_words.Length)
                                    {
                                        for (int indx = 0; indx < str_words.Length; indx++)
                                        {
                                            if (str_words[indx] != buf_words[indx])
                                            {
                                                goto lalala;
                                            }
                                        }
                                        flag++;
                                        j = 3;
                                        break;
                                    }
                                    if (buf_words.Length < str_words.Length)
                                    {
                                        continue;
                                    }
                                    for (int indx = 0; indx < buf_words.Length - str_words.Length + 1; indx++)
                                    {
                                        if (buf_words[indx] == str_words[0])
                                        {
                                            for (int I = 0; I < str_words.Length; I++)
                                            {
                                                if (str_words[I] != buf_words[indx + I])
                                                {
                                                    goto сontinue;
                                                }
                                            }
                                            flag++;
                                            j = 3;
                                        }
                                        goto Break;
                                        сontinue:;
                                    }
                                    lalala:;
                                }
                                Break:
                                if (flag == 0)
                                {
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    continue;
                                }
                                else
                                {
                                    countOfResults++;
                                    flag = 0;
                                }
                            }
                        }
                        else
                        {
                            if (inf.numberOfColumn != 2)
                            {
                                for (int i = inf.first_index; i < inf.second_index; i++)
                                {
                                    buf_words = DataUsers.Rows[i].Cells[inf.numberOfColumn].Value.ToString().ToLower().Split(' ');
                                    if (buf_words.Length == str_words.Length)
                                    {
                                        for (int indx = 0; indx < str_words.Length; indx++)
                                        {
                                            if (str_words[indx] != buf_words[indx])
                                            {
                                                Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                                goto lalala;
                                            }
                                        }
                                        countOfResults++;
                                        continue;
                                    }
                                    if (buf_words.Length < str_words.Length)
                                    {
                                        Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                        continue;
                                    }

                                    for (int indx = 0; indx < buf_words.Length - str_words.Length + 1; indx++)
                                    {
                                        if (buf_words[indx] == str_words[0])
                                        {
                                            for (int I = 0; I < str_words.Length; I++)
                                            {
                                                if (str_words[I] != buf_words[indx + I])
                                                {
                                                    goto сontinue;
                                                }
                                            }
                                            flag++;
                                            countOfResults++;

                                        }
                                        break;
                                        сontinue:;
                                    }
                                    if (flag == 0)
                                        Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    flag = 0;
                                    lalala:;
                                }
                            }
                            else
                            {
                                for(int i = inf.first_index; i < inf.second_index; i++)
                                {
                                    bufferWord = DataUsers.Rows[i].Cells[2].Value.ToString().ToLower();
                                    if(bufferWord != str)
                                        Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                    else
                                        countOfResults++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (inf.numberOfColumn == 5)
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    bufferWord = DataUsers.Rows[i].Cells[j].Value.ToString().ToLower();
                                    if (bufferWord.Length < str.Length) continue;
                                    if (!bufferWord.Contains(str)) continue;
                                    flag++;
                                    countOfResults++;
                                    break;
                                }
                                if (str == DataUsers.Rows[i].Cells[2].Value.ToString()) flag++;
                                if (flag == 0)
                                    Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                else
                                    flag = 0;
                            }
                        }
                        else
                        {
                            if (inf.numberOfColumn != 2)
                            {
                                for (int i = inf.first_index; i < inf.second_index; i++)
                                {
                                    bufferWord = DataUsers.Rows[i].Cells[inf.numberOfColumn].Value.ToString().ToLower();
                                    if (bufferWord.Length < str.Length)
                                    {
                                        Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                        continue;
                                    }
                                    if (!bufferWord.Contains(str))
                                    {
                                        Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                        continue;
                                    }
                                    countOfResults++;
                                }
                            }
                            else
                            {
                                for (int i = inf.first_index; i < inf.second_index; i++)
                                {
                                    bufferWord = DataUsers.Rows[i].Cells[2].Value.ToString();
                                    if (bufferWord != str)
                                    {
                                        Invoke((Action)delegate { DataUsers.Rows[i].Visible = false; });
                                        continue;
                                    }
                                    countOfResults++;
                                }
                            }
                        }
                    }
                }
                Invoke((Action)delegate { countOfTrueRows += countOfResults; });
            });
        }
    }
}
