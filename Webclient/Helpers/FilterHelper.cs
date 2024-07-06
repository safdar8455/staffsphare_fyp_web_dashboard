using Ems.BusinessTracker.Common;
using Ems.BusinessTracker.Common.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Webclient.Models;

namespace Webclient.Helpers
{
    public class FilterHelper
    {
        public static List<T> JQGridFilter<T>(IQueryable<T> query, GridSettings grid)
        {
            //filtring
            if (grid.IsSearch)
            {
                //And
                if (grid.Where.groupOp == "AND")
                    query = grid.Where.rules.Aggregate(query, (current, rule) => current.Where(rule.field, rule.data, (WhereOperation)StringEnum.Parse(typeof(WhereOperation), rule.op)));
                else
                {
                    //Or
                    var temp = (new List<T>()).AsQueryable();
                    temp = grid.Where.rules.Select(rule => query.Where(rule.field, rule.data, (WhereOperation)StringEnum.Parse(typeof(WhereOperation), rule.op))).Aggregate(temp, (current, t) => current.Concat(t));
                    //remove repeating records
                    query = temp.Distinct();
                }
            }

            return query.OrderBy(string.Format("{0} {1}", grid.SortColumn, grid.SortOrder)).ToList();
        }

        public static List<T> JQGridPageData<T>(IQueryable<T> query, GridSettings grid)
        {
            return query.Skip((grid.PageIndex - 1) * grid.PageSize).Take(grid.PageSize).ToList();
        }

        public static void SetPagerFilter(GridSettings grid, PagerModel pagerModel)
        {
            if (pagerModel == null)
                return;

            pagerModel.FilterModel = new DataTable();
            pagerModel.FilterModel.Columns.Add("Id", typeof(int));
            pagerModel.FilterModel.Columns.Add("KeyName", typeof(string));
            pagerModel.FilterModel.Columns.Add("Value", typeof(string));
            if (grid.Where != null && grid.Where.rules.Any())
            {
                var rules = grid.Where.rules;
                for (int i = 1; i <= rules.Length; i++)
                {
                    pagerModel.FilterModel.Rows.Add(i, rules[i - 1].field, rules[i - 1].data);
                }
            }
        }
    }
}