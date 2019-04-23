using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using RESTserver.Models;
using System.Collections;
using System.Xml;

namespace RESTserver
{
    public class DataPersistence
    {
        XDocument xDoc = new XDocument();
        int itemsQuantity = 3;
        
        public DataPersistence()
        {
        xDoc = XDocument.Load("items.xml");
        
         itemsQuantity++;
        }

        public ArrayList GetCollection()
        {

            //var result = xDoc.Descendants("Item");
            //var result = xDoc.Descendants("CATALOG");
            ArrayList itemsArr = new ArrayList();

            var result2 = from q in xDoc.Descendants("Item")
                          select new
                          {
                              Id = q.Element("Id").Value,
                              name = q.Element("name").Value,
                              image = q.Element("image").Value
                          };

            foreach (var cd in result2)
            {
                Items itm = new Items();
                itm.Id = Convert.ToInt32(cd.Id);
                itm.name = Convert.ToString(cd.name);
                itm.image =Convert.ToString(cd.image);
                itemsArr.Add(itm);
            }
            return itemsArr;
        }

        public Items GetItem(int id)
        {

            Items itm = new Items();
            string needId = Convert.ToString(id);

            var node = xDoc.Descendants("Item").FirstOrDefault(cd => cd.Element("Id").Value == needId);
            // If Element not exists
            try
            {
                itm.Id = Convert.ToInt32(node.Element("Id").Value);
                itm.name = node.Element("name").Value;
                itm.image = node.Element("image").Value;
            }
            catch
            {
                itm.Id = 0;
                itm.name = "Element not exists";
                itm.image = null;
            }
            return itm;
        }

        public bool DeleteItem(int id)
        {

            string needId = Convert.ToString(id);
            try
            {
                xDoc.Descendants("Item").FirstOrDefault(cd => cd.Element("Id").Value == needId).Remove();
                xDoc.Save("items.xml");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int PostItem (Items value)
        {
            var doc = new XElement(
                new XElement("Item",
                new XElement("Id", value.Id),
                new XElement("name", value.name),
                new XElement("image", value.image)));


            xDoc.Root.Add(doc);
            xDoc.Save("items.xml");
            return value.Id;
        }

        public bool UpdateItem(int id, Items itm)
        {
            bool elementExist = false;
            string needId = Convert.ToString(id);

            var node = xDoc.Descendants("Item").FirstOrDefault(cd => cd.Element("Id").Value == needId);
            // If Element not exists
            try
            {
                node.Element("Id").Value = itm.Id.ToString();
                node.Element("name").Value = itm.name.ToString();
                node.Element("image").Value = itm.image.ToString();
                elementExist = true;
            }
            catch
            {
                elementExist = false;
            }
            xDoc.Save("items.xml");
            return elementExist;
        }
    }
}