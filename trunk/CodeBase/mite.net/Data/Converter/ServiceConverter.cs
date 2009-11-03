//-----------------------------------------------------------------------
// <copyright>
// This software is licensed as Microsoft Public License (Ms-PL).
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Mite
{
    internal class ServiceConverter : IEntityConverter<Service>
    {

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public string Convert(Service item)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);

            xmlWriter.WriteStartElement("service");

            xmlWriter.WriteElementString("name", item.Name);
            xmlWriter.WriteElementString("note", item.Note);

            xmlWriter.WriteElementString("hourly-rate", item.HourlyRate.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteElementString("archived", item.Archived.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
            xmlWriter.WriteElementString("billable", item.Billable.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());

            if ( item.Id != 0 )
            {
                xmlWriter.WriteElementString("id", item.Id.ToString(CultureInfo.InvariantCulture));
            }

            if ( item.CreatedOn != DateTime.MinValue )
            {
                xmlWriter.WriteElementString("created-at", item.CreatedOn.ToString(CultureInfo.InvariantCulture));
            }

            if ( item.UpdatedOn != DateTime.MinValue )
            {
                xmlWriter.WriteElementString("updated-at", item.UpdatedOn.ToString(CultureInfo.InvariantCulture));
            }

            xmlWriter.WriteEndElement();

            xmlWriter.Close();

            return stringBuilder.ToString();
        }

        public Service Convert(string data)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(data);

            Service service = new Service
                                  {
                                      Id = int.Parse(xmlDocument.SelectSingleNode("/service/id").InnerText, CultureInfo.InvariantCulture),
                Archived = bool.Parse(xmlDocument.SelectSingleNode("/service/archived").InnerText),
                                      CreatedOn = DateTime.Parse(xmlDocument.SelectSingleNode("/service/created-at").InnerText, CultureInfo.InvariantCulture),
                Name = xmlDocument.SelectSingleNode("/service/name").InnerText,
                Note = xmlDocument.SelectSingleNode("/service/note").InnerText,
                                      UpdatedOn = DateTime.Parse(xmlDocument.SelectSingleNode("/service/updated-at").InnerText, CultureInfo.InvariantCulture),
                Billable = bool.Parse(xmlDocument.SelectSingleNode("/service/billable").InnerText)
            };

            int rate = 0;

            int.TryParse(xmlDocument.SelectSingleNode("/service/hourly-rate").InnerText, out rate);

            service.HourlyRate = rate;

            return service;
        }

        public IList<Service> ConvertToList(string data)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(data);

            XmlNodeList nodeList = xmlDocument.SelectNodes(@"/services/service");

            IList<Service> projects = new List<Service>(nodeList.Count);

            foreach ( XmlNode node in nodeList )
            {
                projects.Add(Convert(node.OuterXml));
            }

            return projects;
        }
    }
}