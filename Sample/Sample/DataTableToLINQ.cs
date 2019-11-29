using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sample
{
    internal class DataTableToLINQ
    {
        public void Show()
        {
            //获取历史帐单
            var dt = GetHistoryBillList();
            //迴圈寫法
            var result1 = Process1(dt);
            foreach (var r in result1)
                Console.WriteLine($"{r.DrawDatetime},{r.BetCount},{r.BetMoney},{r.ProfitLossMoney}");
            //改用LINQ
            var result2 = Process2(dt);
            foreach (var r in result2)
                Console.WriteLine($"{r.DrawDatetime},{r.BetCount},{r.BetMoney},{r.ProfitLossMoney}");
            Console.ReadKey();
        }

        private static List<BillViewModel> Process1(DataTable dt)
        {
            //依日期加总并整理历史帐单的内容到综合报表栏位
            var billList = new List<BillViewModel>();
            for (var i = 0; i < dt.Rows.Count;)
            {
                var betCount = 0;
                decimal betMoney = 0;
                decimal profitLossMoney = 0;
                var drawDatetime = dt.Rows[i]["draw_datetime"].ToString();

                for (; i < dt.Rows.Count;)
                    if (drawDatetime == dt.Rows[i]["draw_datetime"].ToString())
                    {
                        //还没结算的不算入统计
                        if (dt.Rows[i]["profit_loss_money"].ToString() != "0")
                        {
                            betCount += Convert.ToInt32(dt.Rows[i]["bet_count"]);
                            betMoney += Convert.ToInt32(dt.Rows[i]["bet_money"]);
                            profitLossMoney += Convert.ToInt32(dt.Rows[i]["profit_loss_money"]);
                        }

                        i++;
                    }
                    else
                    {
                        break;
                    }

                var historyBillData = new BillViewModel
                {
                    BetCount = betCount,
                    BetMoney = betMoney,
                    DrawDatetime = drawDatetime,
                    ProfitLossMoney = profitLossMoney
                };

                //-1是全部合计不需要回传综合报表
                if (drawDatetime != "-1") billList.Add(historyBillData);
            }

            return billList;
        }

        private static List<BillViewModel> Process2(DataTable dt)
        {
            var billList = new List<BillViewModel>();
            var list = dt.AsEnumerable().Select(row => new ViewModel(row)).ToList();

            var result = from h in list
                where h.ProfitLossMoney != 0 //还没结算的不算入统计及
                      && h.DrawDatetime != "-1" //-1是全部合计不需要回传综合报表
                group h by h.DrawDatetime
                into g
                select new BillViewModel
                {
                    BetCount = g.Sum(p => p.BetCount),
                    BetMoney = g.Sum(p => p.BetMoney),
                    DrawDatetime = g.Select(p => p.DrawDatetime).FirstOrDefault(),
                    ProfitLossMoney = g.Sum(p => p.ProfitLossMoney)
                };
            billList.AddRange(result);
            return billList;
        }

        private static DataTable GetHistoryBillList()
        {
            var table = new DataTable();
            table.Columns.Add("draw_datetime", typeof(string));
            table.Columns.Add("bet_count", typeof(int));
            table.Columns.Add("bet_money", typeof(int));
            table.Columns.Add("profit_loss_money", typeof(decimal));

            table.Rows.Add("2019-11-20", 1, 100, 0);
            table.Rows.Add("2019-11-20", 2, 200, 1000);
            table.Rows.Add("2019-11-21", 1, 100, 300);
            return table;
        }
    }

    internal class ViewModel
    {
        public ViewModel(DataRow row)
        {
            DrawDatetime = row.Field<string>("draw_datetime");
            BetCount = row.Field<int>("bet_count");
            BetMoney = row.Field<int>("bet_money");
            ProfitLossMoney = row.Field<decimal>("profit_loss_money");
        }

        public string DrawDatetime { get; set; }

        public int BetCount { get; set; }

        public decimal BetMoney { get; set; }

        public decimal ProfitLossMoney { get; set; }
    }

    internal class BillViewModel
    {
        public int BetCount { get; set; }
        public decimal BetMoney { get; set; }
        public string DrawDatetime { get; set; }
        public decimal ProfitLossMoney { get; set; }
    }
}