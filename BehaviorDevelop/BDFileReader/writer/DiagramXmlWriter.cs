using BDFileReader.util;
using BDFileReader.vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BDFileReader.writer
{
    public class DiagramXmlWriter
    {

        public static void outputDiagramXml(DiagramVO diagvo, int depth, StreamWriter sw)
        {

            sw.Write(StringUtil.indent(depth) + "<diagram ");
            if (diagvo.changed != ' ')
            {
                sw.Write("changed=\"" + diagvo.changed + "\" ");
            }
            sw.Write("guid=\"" + StringUtil.escapeXML(diagvo.guid) + "\" ");
            sw.Write("tpos=\"" + diagvo.treePos + "\" ");
            sw.Write("type=\"" + diagvo.diagramType + "\" ");
            sw.Write("name=\"" + StringUtil.escapeXML(diagvo.name) + "\" ");
            sw.Write("showDetails=\"" + diagvo.showDetails + "\" ");
            sw.Write("attPub=\"" + diagvo.attPub + "\" ");
            sw.Write("attPri=\"" + diagvo.attPri + "\" ");
            sw.Write("attPro=\"" + diagvo.attPro + "\" ");
            sw.Write("cx=\"" + diagvo.cx + "\" ");
            sw.Write("cy=\"" + diagvo.cy + "\" ");
            sw.Write("scale=\"" + diagvo.scale + "\" ");
            sw.Write("orientation=\"" + diagvo.orientation + "\" ");
            sw.Write("createdDate=\"" + diagvo.createdDate + "\" ");
            sw.Write("swimlanes=\"" + diagvo.swimlanes + "\"");
            sw.WriteLine(">");

            if (diagvo.notes != null)
            {
                sw.WriteLine(StringUtil.indent(depth + 1) + "<notes>" + StringUtil.escapeXML(diagvo.notes) + "</notes>");
            }

            if (diagvo.diagramObjects != null)
            {
                foreach (DiagramObjectVO diaObjVO in diagvo.diagramObjects)
                {
                    outputDiagramObject(diaObjVO, depth+1, sw);
                }
            }

            if (diagvo.diagramLinks != null)
            {
                foreach (DiagramLinkVO diaLinkVO in diagvo.diagramLinks)
                {
                    outputDiagramLink(diaLinkVO, depth+1, sw);
                }
            }
            sw.WriteLine(StringUtil.indent(depth) + "</diagram>");
        }

        private static void outputDiagramObject(DiagramObjectVO diaObjVO, int depth, StreamWriter sw)
        {
            sw.Write(StringUtil.indent(depth) + "<diagramObject ");
            if (diaObjVO.changed != ' ')
            {
                sw.Write("changed=\"" + diaObjVO.changed + "\" ");
            }

            sw.Write("sequence=\"" + diaObjVO.sequence + "\" ");
            sw.Write("objectId=\"" + diaObjVO.objectId + "\" ");
            sw.Write("rectTop=\"" + diaObjVO.rectTop + "\" ");
            sw.Write("rectLeft=\"" + diaObjVO.rectLeft + "\" ");
            sw.Write("rectRight=\"" + diaObjVO.rectRight + "\" ");
            sw.Write("rectBottom=\"" + diaObjVO.rectBottom + "\" ");
            sw.Write("instanceId=\"" + diaObjVO.instanceId + "\" ");
            sw.Write("objectStyle=\"" + StringUtil.escapeXML(diaObjVO.objectStyle) + "\" ");
            sw.WriteLine(" />");
        }

        private static void outputDiagramLink(DiagramLinkVO diaLinkVO, int depth, StreamWriter sw)
        {
            sw.Write(StringUtil.indent(depth) + "<diagramLink ");
            if (diaLinkVO.changed != ' ')
            {
                sw.Write("changed=\"" + diaLinkVO.changed + "\" ");
            }

            sw.Write("connectorId=\"" + diaLinkVO.connectorId + "\" ");
            sw.Write("hidden=\"" + diaLinkVO.hidden + "\" ");
            sw.Write("geometry=\"" + StringUtil.escapeXML(diaLinkVO.geometry) + "\" ");
            sw.Write("style=\"" + StringUtil.escapeXML(diaLinkVO.style) + "\" ");
            sw.WriteLine(" />");
        }

    }
}
