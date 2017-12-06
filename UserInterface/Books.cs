using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MetroFramework;

namespace UserInterface
{
    public partial class Books : Form
    {
        private List< List<DataGridViewRow> > Users;
        private bool Lock_Flag { get; set; }
        private int countOfTrueRows = 0;
        public DataGridView DataBooksGrid => DataBooks;
        private Search_form sf;
        public DataGridView DataCurrentBook => DataBooksInf;
        private DataGridViewRow item;
        private Users friend;
        public Books(Users friend)
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            Users = new List<List<DataGridViewRow>>();
            InitializeComponent();
            Lock_Flag = false;
            Delete.Enabled = false;
            this.friend = friend;
            MoveObj.BackColor = System.Drawing.Color.Transparent;
            CLOSE.FlatAppearance.BorderSize = 0;
            MINIMASE.FlatAppearance.BorderSize = 0;
        }

        #region Анимация
        private int CS_DROPSHADOW = 0x00020000;
        
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        } // для тени формы
        private void MoveObj_MouseDown(object sender, MouseEventArgs e)
        {
            MoveObj.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref m);
        }
        private void MINIMASE_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void CLOSE_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Books_Deactivate(object sender, EventArgs e)
        {
            MoveObj.Focus();
        }
        private void MINIMASE_MouseMove(object sender, MouseEventArgs e)
        {
            MINIMASE.BackColor = Color.Silver;
        }
        private void MINIMASE_MouseLeave(object sender, EventArgs e)
        {
            MINIMASE.BackColor = Color.Gainsboro;
        }
        private void CLOSE_MouseMove(object sender, MouseEventArgs e)
        {
            CLOSE.BackColor = Color.Crimson;
            CLOSE.ForeColor = Color.White;
        }
        private void CLOSE_MouseLeave(object sender, EventArgs e)
        {
            CLOSE.BackColor = Color.Gainsboro;
            CLOSE.ForeColor = Color.Black;
        }
        #endregion

        #region Служебные методы, облегчающие разработку
        private (string, string) GetDataRow(DataGridViewRow row)
        {
            return (row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
        }
        public void AddToDATA(string first, string second, string third, string fourth)
        {
            int n = DataBooks.Rows.Add();
            DataBooks.Rows[n].Cells[0].Value = first;
            DataBooks.Rows[n].Cells[1].Value = second;
            DataBooks.Rows[n].Cells[2].Value = third;
            DataBooks.Rows[n].Cells[3].Value = fourth;
            Users.Add(new List<DataGridViewRow>());
        } // это метод добавления пользователя или книги
        public void LoadMainData(Object t)
        {
            DataSet dataalll = new DataSet();
            dataalll.ReadXml("BD.xml");
            try
            {
                int n;
                foreach (DataRow row in dataalll.Tables["Books"].Rows)
                {
                    n = DataBooks.Rows.Add(); // добавляем новую сроку в dataGridView1
                    DataBooks.Rows[n].Cells[0].Value = row["BooksName"]; // заносим в первый столбец созданной строки данные из первого столбца таблицы ds.
                    DataBooks.Rows[n].Cells[1].Value = row["Author"]; // то же самое со вторым столбцом
                }
            }
            catch
            {
                return;
            }
            // ((AutoResetEvent)t).Set();
        }
        public void RemoveingUser(List<DataGridViewRow> list, int index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < Users[list[i].Index].Count; j++)
                {
                    if (Users[list[i].Index][j].Index == index)
                    {
                        Users[list[i].Index].RemoveAt(j);
                    }
                }
            }

        }
        public void RemoveingBook(int IndexUser, int IndexBook)
        {
            for (int i = 0; i < Users[IndexBook].Count; i++)
            {
                if (Users[IndexBook][i].Index == IndexUser)
                {
                    this.Users[IndexBook].RemoveAt(i);
                    break;
                }
            }
        }
        public void UpdateDataBooksInf()
        {
            if(DataBooks.SelectedRows.Count != 0)
                DataBooks_SelectionChanged(1, new EventArgs());
        }
        public void AddUserDrDr(DataGridViewRow row)
        {
            Users[DataBooks.CurrentRow.Index].Add(row);
            int n = DataBooksInf.Rows.Add();
            DataBooksInf.Rows[n].Cells[0].Value = Users[DataBooks.CurrentRow.Index][Users[DataBooks.CurrentRow.Index].Count - 1].Cells[0].Value.ToString();
            DataBooksInf.Rows[n].Cells[1].Value = Users[DataBooks.CurrentRow.Index][Users[DataBooks.CurrentRow.Index].Count - 1].Cells[1].Value.ToString();
        }
        #endregion

        private void AddTooo_Click(object sender, EventArgs e)
        {
            AddPoly addpoly = new AddPoly(this);
            addpoly.ShowDialog();
        }
        private void DataBooks_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    case MouseButtons.Left when (DataBooks.CurrentRow.Selected):
                        DataBooksInf.Rows.Clear();
                        for (int i = 0; i < Users[DataBooks.CurrentRow.Index].Count; i++)
                        {
                            int n = DataBooksInf.Rows.Add();
                            (DataBooksInf.Rows[n].Cells[0].Value, DataBooksInf.Rows[n].Cells[1].Value) = GetDataRow(Users[DataBooks.CurrentRow.Index][i]);
                        }
                        DataBooksInf.ClearSelection();
                        Delete.Enabled = true;
                        Delete.Text = "Удалить книгу";
                        break;
                    case MouseButtons.Middle:
                        for (int i = 0; i < DataBooks.RowCount; i++)
                        {
                            DataBooks.Rows[i].Visible = true;
                        }
                        DataBooks.ClearSelection();
                        DataBooksInf.Rows.Clear();
                        Delete.Text = "Удалить...";
                        Delete.Enabled = false;
                        InfoLabelBookUserCount.Visible = InfoLabelBookUserCountValue.Visible = false;
                        break;
                }
            }
            catch { }
        }
        private void Delete_Click(object sender, EventArgs e) /////////////////////////////////////// сделать удаление элемента кнопкой
        {
            if (Delete.Text == "Удалить книгу")
            {
                DataBooksInf.Rows.Clear();
                friend.RemoveingBook(Users[DataBooks.CurrentRow.Index], DataBooks.CurrentRow.Index);
                Users.RemoveAt(DataBooks.CurrentRow.Index);
                DataBooks.Rows.RemoveAt(DataBooks.CurrentRow.Index);
                if (DataBooks.Rows.Count == 0)
                {
                    Delete.Enabled = false;
                    Delete.Text = "Удалить...";
                }
                CountOfBooksTotal.Text = DataBooks.RowCount.ToString();
            }
            else
            {
                friend.RemoveingUser(DataBooks.CurrentRow.Index, Users[DataBooks.CurrentRow.Index][DataBooksInf.CurrentRow.Index].Index);
                Users[DataBooks.CurrentRow.Index].RemoveAt(DataBooksInf.CurrentRow.Index);
                DataBooksInf.Rows.RemoveAt(DataBooksInf.CurrentRow.Index);
                if (DataBooksInf.Rows.Count == 0)
                {
                    Delete.Enabled = false;
                    Delete.Text = "Удалить...";
                }
            }
            friend.UpdateDataUserInf();
            //Delete.Enabled = false;
            //Delete.Text = "Удалить...";
        }
        private void Books_Load(object sender, EventArgs e)
        {
            DataSet dataalll = new DataSet();
            dataalll.ReadXml("BD.xml");
            try
            {
                int i = 0;
                int n = 0;
                foreach (DataRow row in dataalll.Tables["Books"].Rows)
                {
                    DataBooks.Rows[n].Cells[2].Value = row["Genre"]; // то же самое с третьим столбцом
                    DataBooks.Rows[n++].Cells[3].Value = row["YearPublication"]; // то же самое с четвертым столбцом
                    string[] indexes = row["IndexesOfCurrentUsers"].ToString().Split('.');
                    foreach (string index in indexes)
                    {
                        Users.Add(new List<DataGridViewRow>());
                        try
                        {
                            Users[i].Add(friend.DataUserGrid.Rows[Convert.ToInt32(index)]);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    i++;
                }
                DataBooks.ClearSelection();
                MoveObj.Focus();
            }
            catch (NullReferenceException)
            {
                return;
            }
            this.DataBooks.SelectionChanged += new System.EventHandler(this.DataBooks_SelectionChanged);

            CountOfBooksTotal.Text = DataBooks.RowCount.ToString();
        }
        private void DataBooksInf_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        DataBooksInf.ClearSelection();
                        Delete.Enabled = false;
                        Delete.Text = "Удалить...";
                        break;
                    case MouseButtons.Left when (DataBooksInf.SelectedRows.Count == 1):
                        Delete.Enabled = true;
                        Delete.Text = "Убрать пользователя";
                        break;
                }
            }
            catch { }
        }
        private void DataBooks_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DataBooksInf.Rows.Clear();
            friend.RemoveingBook(Users[e.Row.Index], e.Row.Index);
            Users.RemoveAt(e.Row.Index);
            friend.UpdateDataUserInf();
            CountOfBooksTotal.Text = (DataBooks.RowCount - 1).ToString();
        }
        private void DataBooksInf_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            friend.RemoveingUser(DataBooks.CurrentRow.Index, Users[DataBooks.CurrentRow.Index][e.Row.Index].Index);
            Users[DataBooks.CurrentRow.Index].RemoveAt(e.Row.Index);
            if (DataBooksInf.Rows.Count == 1)
            {
                Delete.Enabled = false;
                Delete.Text = "Удалить...";
            }
            friend.UpdateDataUserInf();
        }
        private void DataBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (DataBooks.RowCount == 0)
            {
                Delete.Enabled = false;
                Delete.Text = "Удалить...";
                return;
            }
            DataBooksInf.Rows.Clear();
            for (int i = 0; i < Users[DataBooks.CurrentRow.Index].Count; i++)
            {
                int n = DataBooksInf.Rows.Add();
                (DataBooksInf.Rows[n].Cells[0].Value, DataBooksInf.Rows[n].Cells[1].Value) = GetDataRow(Users[DataBooks.CurrentRow.Index][i]);
            }
            DataBooksInf.ClearSelection();
            InfoLabelBookUserCount.Visible = InfoLabelBookUserCountValue.Visible = true;
            InfoLabelBookUserCountValue.Text = DataBooksInf.RowCount.ToString();
            Delete.Enabled = true;
            Delete.Text = "Удалить книгу";
        }
        private void DataBooks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Right || DataBooks.RowCount == 0 || !DataBooks.CurrentRow.Selected || DataBooksInf.RowCount == 0) return;
            DataBooksInf.Rows[0].Selected = true;
            DataBooksInf.Focus();
            Delete.Text = "Убрать пользователя";
        }
        private void DataBooksInf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left) return;
            DataBooks.Focus();
            Delete.Text = "Удалить книгу";
            Delete.Enabled = true;
            DataBooksInf.ClearSelection();
        }
        private void DataBooksInf_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            item = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
        }
        private void DataBooksInf_DragDrop(object sender, DragEventArgs e)
        {
            for(int i = 0; i < DataBooksInf.Rows.Count; i++)
                if(DataBooksInf.Rows[i].Cells[0].Value.ToString() == item.Cells[0].Value.ToString() && DataBooksInf.Rows[i].Cells[1].Value.ToString() == item.Cells[1].Value.ToString())
                {
                    MessageBox.Show("Данный пользователь уже имеется у выбранной Вами книги", "Ошибка добавления");
                    return;
                }
            int n = DataBooksInf.Rows.Add();
            DataBooksInf.Rows[n].Cells[0].Value = item.Cells[0].Value.ToString();
            DataBooksInf.Rows[n].Cells[1].Value = item.Cells[1].Value.ToString();
            Users[DataBooks.CurrentRow.Index].Add(item);
            friend.AddBookDrDr(DataBooks.CurrentRow);
        }
        private void DataBooks_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex == -1) return;
            Lock_Flag = false;
            DataBooks.CurrentCell = DataBooks.Rows[e.RowIndex].Cells[0];///////////////////////////
            DataBooks.CurrentRow.Selected = true;
            Lock_Flag = true;
            DataBooks_SelectionChanged(1, new EventArgs());
            try { if (!friend.DataUserGrid.CurrentRow.Selected) return; }
            catch { return; }
            friend.DataCurrentUser.AllowDrop = true;
            friend.DataCurrentUser.DoDragDrop(DataBooks.CurrentRow, DragDropEffects.All);
            friend.DataCurrentUser.AllowDrop = false;
        }
        private void Books_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataSet dataalll = new DataSet();
            dataalll.ReadXml("BD.xml");
            try { dataalll.Tables.Remove("Books"); }
            catch { }
            DataTable dt = new DataTable();
            dt.TableName = "Books";
            dt.Columns.Add("BooksName");
            dt.Columns.Add("Author");
            dt.Columns.Add("Genre");
            dt.Columns.Add("YearPublication");
            dt.Columns.Add("IndexesOfCurrentUsers");
            dataalll.Tables.Add(dt);
            foreach (DataGridViewRow row in DataBooks.Rows)
            {
                DataRow data = dataalll.Tables["Books"].NewRow();
                data["BooksName"] = row.Cells[0].Value.ToString();
                data["Author"] = row.Cells[1].Value.ToString();
                data["Genre"] = row.Cells[2].Value.ToString();
                data["YearPublication"] = row.Cells[3].Value.ToString();
                string indexesofbooks = "";
                for (int i = 0; i < Users[row.Index].Count; i++)
                    indexesofbooks += Users[row.Index][i].Index.ToString() + '.';
                data["IndexesOfCurrentUsers"] = indexesofbooks;
                dataalll.Tables["Books"].Rows.Add(data);
            }
            dataalll.WriteXml("BD.xml");
        }
        private void Search_Click(object sender, EventArgs e)
        {
            sf = new Search_form(this);
            sf.ShowDialog();
        }
        public void Searching(SearchData data)
        {
            if (data.UseMultiThreading)
            {
                if (DataBooks.RowCount < Environment.ProcessorCount * 2)
                {
                    ThreadPool.QueueUserWorkItem(arg => ThreadSearch(data));
                    return;
                }
                int first_index = 0, second_index, step;
                Thread[] threads = new Thread[Environment.ProcessorCount * 2];
                SearchData[] datas = new SearchData[threads.Length];
                step = DataBooks.RowCount / datas.Length;
                second_index = step;
                for (int i = 0; i < datas.Length; i++)
                {
                    datas[i] = new SearchData();
                    datas[i] = data;
                    datas[i].first_index = first_index;
                    datas[i].second_index = second_index;
                    threads[i] = new Thread(ThreadSearch);
                    first_index += step;
                    second_index += step;
                }
                if (second_index < DataBooks.RowCount)
                {
                    data.first_index = second_index;
                    data.second_index = DataBooks.RowCount;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadSearch), data);
                }
                for (int i = 0; i < datas.Length - 1; i++)
                {
                    threads[i].Start(datas[i]);
                }
                threads[datas.Length - 1].Name = "LAST";
                threads[datas.Length - 1].Start(datas[datas.Length - 1]);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(arg => ThreadSearch(data));
            }

        }
        private void ThreadSearch(Object data)
        {
            SearchData inf = data as SearchData;
            string str = inf.str;
            int countOfResults = 0;
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
                                buf_words = DataBooks.Rows[i].Cells[j].Value.ToString().Split(' ');
                                if (buf_words.Length == str_words.Length)
                                {
                                    for (int indx = 0; indx < str_words.Length; indx++)
                                    {
                                        if (str_words[indx] != buf_words[indx])
                                        {
                                            Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                            goto lalala;
                                        }
                                    }
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    countOfResults++;
                                    j = 3;
                                    continue;
                                }
                                if (buf_words.Length < str_words.Length)
                                {
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                    continue;
                                }
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
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
                                        Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                        countOfResults++;
                                        j = 3;
                                        break;
                                        сontinue:;
                                    }
                                }
                                lalala:;
                            }
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
                        }
                    }
                    else
                    {
                        for (int i = inf.first_index; i < inf.second_index; i++)
                        {
                            buf_words = DataBooks.Rows[i].Cells[inf.numberOfColumn].Value.ToString().Split(' ');
                            if (buf_words.Length == str_words.Length)
                            {
                                for (int indx = 0; indx < str_words.Length; indx++)
                                {
                                    if (str_words[indx] != buf_words[indx])
                                    {
                                        Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                        goto lalala;
                                    }
                                }
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                countOfResults++;
                                continue;
                            }
                            if (buf_words.Length < str_words.Length)
                            {
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                continue;
                            }
                            Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
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
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    countOfResults++;
                                    break;
                                    сontinue:;
                                }
                            }
                            lalala:;
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
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
                                bufferWord = DataBooks.Rows[i].Cells[j].Value.ToString();
                                if (bufferWord.Length < str.Length) continue;
                                if (bufferWord.Contains(str))
                                {
                                    countOfResults++;
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    break;
                                }
                                else
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                            }
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
                        }
                    }
                    else
                    {
                        for (int i = inf.first_index; i < inf.second_index; i++)
                        {
                            bufferWord = DataBooks.Rows[i].Cells[inf.numberOfColumn].Value.ToString();
                            if (bufferWord.Length < str.Length) continue;
                            if (bufferWord.Contains(str))
                            {
                                countOfResults++;
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                continue;
                            }
                            else
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
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
                                buf_words = DataBooks.Rows[i].Cells[j].Value.ToString().ToLower().Split(' ');
                                if (buf_words.Length == str_words.Length)
                                {
                                    for (int indx = 0; indx < str_words.Length; indx++)
                                    {
                                        if (str_words[indx] != buf_words[indx])
                                        {
                                            Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                            goto lalala;
                                        }
                                    }
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    countOfResults++;
                                    j = 3;
                                    continue;
                                }
                                if (buf_words.Length < str_words.Length)
                                {
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                    continue;
                                }
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
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
                                        Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                        countOfResults++;
                                        j = 3;
                                        break;
                                        сontinue:;
                                    }
                                }
                                lalala:;
                            }
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
                        }
                    }
                    else
                    {
                        for (int i = inf.first_index; i < inf.second_index; i++)
                        {
                            buf_words = DataBooks.Rows[i].Cells[inf.numberOfColumn].Value.ToString().Split(' ');
                            if (buf_words.Length == str_words.Length)
                            {
                                for (int indx = 0; indx < str_words.Length; indx++)
                                {
                                    if (str_words[indx] != buf_words[indx].ToLower())
                                    {
                                        Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                        goto lalala;
                                    }
                                }
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                countOfResults++;
                                continue;
                            }
                            if (buf_words.Length < str_words.Length)
                            {
                                Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                                continue;
                            }
                            Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                            for (int indx = 0; indx < buf_words.Length - str_words.Length + 1; indx++)
                            {
                                if (buf_words[indx].ToLower() == str_words[0])
                                {
                                    for (int I = 0; I < str_words.Length; I++)
                                    {
                                        if (str_words[I] != buf_words[indx + I].ToLower())
                                        {
                                            goto сontinue;
                                        }
                                    }
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    countOfResults++;
                                    break;
                                    сontinue:;
                                }
                            }
                            lalala:;
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
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
                            for (int j = 0; j < 3; j++)
                            {
                                bufferWord = DataBooks.Rows[i].Cells[j].Value.ToString().ToLower();
                                if (bufferWord.Length < str.Length) continue;
                                if (bufferWord.Contains(str))
                                {
                                    countOfResults++;
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    break;
                                }
                                else
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                            }
                        }
                        if (!inf.UseMultiThreading)
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                            Invoke((Action)delegate { sf.Close(); }); //////////////////////
                        }
                        else
                        {
                            Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                            if (Thread.CurrentThread.Name == "LAST")
                            {
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
                        }
                    }
                    else
                    {
                        if (inf.numberOfColumn != 2)
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                bufferWord = DataBooks.Rows[i].Cells[inf.numberOfColumn].Value.ToString().ToLower();
                                if (bufferWord.Length < str.Length) continue;
                                if (bufferWord.Contains(str))
                                {
                                    countOfResults++;
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    continue;
                                }
                                else
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                            }
                            if (!inf.UseMultiThreading)
                            {
                                Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
                            else
                            {
                                Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                                if (Thread.CurrentThread.Name == "LAST")
                                {
                                    Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                    Invoke((Action)delegate { sf.Close(); }); //////////////////////
                                }
                            }
                        }
                        else
                        {
                            for (int i = inf.first_index; i < inf.second_index; i++)
                            {
                                bufferWord = DataBooks.Rows[i].Cells[inf.numberOfColumn].Value.ToString().ToLower();
                                if (bufferWord == str)
                                {
                                    countOfResults++;
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = true; });
                                    continue;
                                }
                                else
                                    Invoke((Action)delegate { DataBooks.Rows[i].Visible = false; });
                            }
                            if (!inf.UseMultiThreading)
                            {
                                Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                                Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                Invoke((Action)delegate { sf.Close(); }); //////////////////////
                            }
                            else
                            {
                                Invoke((Action)delegate { countOfTrueRows += countOfResults; });
                                if (Thread.CurrentThread.Name == "LAST")
                                {
                                    Invoke((Action)delegate { MessageBox.Show("Совпадений: " + countOfTrueRows.ToString(), "Поиск завершен"); });
                                    Invoke((Action)delegate { sf.Close(); }); //////////////////////
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}