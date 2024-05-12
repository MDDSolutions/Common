using MDDDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public class ctlDataEditor<T> : UserControl where T : DBEntity
    {
        private T entity;
        public ctlDataEditor(T inentity)
        {
            entity = inentity;

            //int cury = 5;
            //int labelx = 5;
            //int labelw = 100;
            //int ctlheight = 20;
            //int yspace = 5;

            foreach (var item in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var l = new Label();
                l.Text = $"{item.Name}:";
                //l.Location
            }
        }
    }
}
