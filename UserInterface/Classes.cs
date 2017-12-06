using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class SearchData
    {
        public SearchData() { }
        public SearchData(int first, int second, int columnNuber, string str, bool UseWordWhole, bool RegistrON, bool UseMultiThreading)
        {
            first_index = first;
            second_index = second;
            numberOfColumn = columnNuber;
            this.str = str;
            this.UseWordWhole = UseWordWhole;
            this.RegistrON = RegistrON;
            this.UseMultiThreading = UseMultiThreading;
        }
        public int first_index { get; set; }
        public int second_index { get; set; }
        public int numberOfColumn { get; set; }
        public string str { get; set; }
        public bool UseWordWhole { get; set; }
        public bool RegistrON { get; set; }
        public bool UseMultiThreading { get; set; }
    }
    static public class Helper
    {
        static public void UnHideAllRows(System.Windows.Forms.DataGridView dataGridView)
        {
            for (int i = 0; i < dataGridView.RowCount; i++) dataGridView.Rows[i].Visible = true;
        }
        static public Task UnHideAllRowsAsync(System.Windows.Forms.DataGridView dataGridView, System.Windows.Forms.Form descriptor)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < dataGridView.RowCount; i++) descriptor.Invoke((Action)delegate { dataGridView.Rows[i].Visible = true; });
            });
        }
        static public Task UnHideAllRowsAsyncUnsafe(System.Windows.Forms.DataGridView dataGridView)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < dataGridView.RowCount; i++) dataGridView.Rows[i].Visible = true;
            });
        }
    }
    public class Searcher
    {
        private delegate void TaskEningSearch(int result);
        static private event TaskEningSearch EndSearchCurrentThread = new TaskEningSearch(IncreaseResult);
        public delegate void SearchEnded(int countOfAppropriateRows);
        static public event SearchEnded SearchEND;
        static int CountOfAppropriateRows = 0;
        static private void IncreaseResult(int number)
        {

        }
        static public async void SearchingAsync(System.Windows.Forms.Form descriptor, System.Windows.Forms.DataGridView dataGridView)
        {
            int CountOfMarksRows = await SearchAnsync();
        }
        static private Task<int> SearchAnsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return 5;
            });
        }

    }
}
