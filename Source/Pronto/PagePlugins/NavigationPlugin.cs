﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Pronto.PagePlugins
{
    public class NavigationPlugin : PagePluginBase
    {
        public override IEnumerable<XObject> Render(string data)
        {
            if (data.StartsWith("level-"))
            {
                var level = int.Parse(data.Substring("level-".Length));
                var p = Website.FindCurrentPageAtLevel(level, Page);
                if (p.Any()) // has child pages?
                {
                    yield return new XElement("ul",
                        new XAttribute("id", "cms-navigation-level-" + level),
                        BuildMenu(p, Page, level)
                    );
                }
            }
            else
            {
                yield return new XElement("ul",
                    new XAttribute("id", "cms-navigation"),
                    BuildMenu(Website, Page, 0)
                );
            }
        }

        IEnumerable<XElement> BuildMenu(IEnumerable<IReadOnlyPage> pages, IReadOnlyPage currentPage, int level)
        {
            return from page in pages
                   where IsUserAdmin || page.Navigation
                   let classes = new[] {
                       page.Path.Replace('/', '_'),
                       "level-" + level, 
                       (page.Path == currentPage.Path || page.Contains(currentPage) ? "current" : "")
                   }
                   select new XElement("li", 
                       (IsUserAdmin ? new XAttribute("data-path", page.Path) : null),
                       new XAttribute("class", string.Join(" ", classes)),
                       (page.Navigation ? null : new XAttribute("style", "display:none")),
                       GetLinkOrSpan(page, currentPage),
                       SubMenu(page, currentPage, level + 1)
                   );
        }

        XElement SubMenu(IReadOnlyPage page, IReadOnlyPage currentPage, int level)
        {
            if (IsUserAdmin || page.Count(c => c.Navigation) > 0)
            {
                return new XElement("ul", BuildMenu(page, currentPage, level));
            }
            else
            {
                return null;
            }
        }

        XElement GetLinkOrSpan(IReadOnlyPage page, IReadOnlyPage currentPage)
        {
            if (page.Path == currentPage.Path)
            {
                return new XElement("span", page.NavigationText);
            }
            else
            {
                return new XElement("a", new XAttribute("href", UrlBase + page.Path), page.NavigationText);
            }
        }
    }
}
