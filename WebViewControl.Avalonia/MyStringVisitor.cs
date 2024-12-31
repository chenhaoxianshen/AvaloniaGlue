using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace WebViewControl {
    class MyStringVisitor : CefStringVisitor {
        private readonly string _contentType;

        public MyStringVisitor(string contentType) {
            _contentType = contentType;
        }

        protected override void Visit(string value) {
             
            Console.WriteLine($"[{_contentType}]");
            Console.WriteLine(value); // 打印获取到的内容
        }
    }

}
