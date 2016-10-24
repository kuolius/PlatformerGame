using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RPGame
{
    public class Items
    {
        [XmlElement("item")]
        public List<Item> itemList = new List<Item>();
        
    }

    public class Item
    {
        [XmlElement("add")]
        public List<Add> addList = new List<Add>();
        [XmlElement("stackable")]
        public bool stackable { get; set; }
        [XmlElement("ranged")]
        public bool ranged { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("type")]
        public string eqType { get; set; }
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("set")]
        public string set { get; set; }

    }

    public class Add
    {
        [XmlAttribute("val")]
        public int val { get; set; }
        [XmlAttribute("stat")]
        public string stat { get; set; }
    }


    
}
