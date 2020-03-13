using System;
using Telerik.XamarinForms.DataGrid;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public class BsdRadDataGrid : RadDataGrid
    {
        public BsdRadDataGrid()
        {
            AutoGenerateColumns = false;
            UserGroupMode = DataGridUserGroupMode.Disabled;
            UserFilterMode = DataGridUserFilterMode.Disabled;
            UserEditMode = DataGridUserEditMode.None;
            GridLinesVisibility = GridLinesVisibility.Both;
            UserSortMode = Telerik.XamarinForms.Common.Data.DataGridUserSortMode.None;

            // cột đậm cột nhạt
            AlternateRowBackgroundStyle = new DataGridBorderStyle()
            {
                BackgroundColor = Color.White,
                BorderColor = Color.Black
            };

            // cột đậm cột nhạt
            RowBackgroundStyle = new DataGridBorderStyle()
            {
                BackgroundColor = Color.FromHex("#eeeeee"),
                BorderColor = Color.Black
            };

            // màu khi chọn
            SelectionStyle = new DataGridBorderStyle()
            {
                BorderColor = Color.Black,
                BorderThickness = 1
            };
        }
    }
}
