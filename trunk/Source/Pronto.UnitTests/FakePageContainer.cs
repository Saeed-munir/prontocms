﻿using System.Collections.Generic;

namespace Pronto
{
    class FakePageContainer : IPageContainer
    {
        public FakePageContainer()
        {
            Path = "";
            Pages = new List<Page>();
        }

        public string Path { get; set; }
        public List<Page> Pages { get; set; }
    }
}
