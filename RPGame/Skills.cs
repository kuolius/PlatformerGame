using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RPGame
{
    public class Skills
    {
        [XmlElement("skill")]
        public List<Skill> skillList = new List<Skill>();
    }

    public class Skill
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("type")]
        public string skillType { get; set; }
        [XmlElement("animationLength")]
        public int animationLength { get; set; }
        [XmlElement("power")]
        public int power { get; set; }
        [XmlElement("range")]
        public int range { get; set; }
        [XmlElement("reuse")]
        public int reuse { get; set; }
        [XmlElement("time")]
        public int time { get; set; }
        [XmlElement("add")]
        public List<Add> add = new List<Add>();
        [XmlElement("fps")]
        public int fps { get; set; }
        [XmlElement("projectile")]
        public bool projectile { get; set; }
    }

   
}
