using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //取得DataTabe資料
            var dt = GetConsumerTable();

            //lambda寫法
            var settlementList = dt.AsEnumerable().Select(row => new CashSettlementModel(row)).Where(col => col.WinMoney > 0 || col.ReturnWater > 0).ToList();
            Console.WriteLine(settlementList);

            //LINQ寫法
            var settlementList1 = (from row in dt.AsEnumerable()
                                                         where row.Field<decimal>("win_money") > 0 || row.Field<decimal>("downline_return_water") > 0
                                                         select new CashSettlementModel(row)).ToList();
            Console.WriteLine(settlementList1);
        }

        public static DataTable GetConsumerTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("win_money", typeof(decimal));
            table.Columns.Add("downline_return_water", typeof(decimal));

            table.Rows.Add(1, "kobe", 10.0, 0);
            table.Rows.Add(2, "jordan", 0.0, 5);
            table.Rows.Add(3, "yao", 0.0, 0);
            return table;
        }
    }

    public class CashSettlementModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal WinMoney { get; set; }
        public decimal ReturnWater { get; set; }

        public CashSettlementModel(DataRow row)
        {
            Id = row.Field<int>("id");
            Name = row.Field<string>("name");
            WinMoney = row.Field<decimal>("win_money");
            ReturnWater = Convert.ToDecimal(row.Field<decimal>("downline_return_water"));
        }
    }
}