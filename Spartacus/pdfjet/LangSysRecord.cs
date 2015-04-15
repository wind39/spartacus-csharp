using System;

namespace PDFjet.NET {
class LangSysRecord {
    byte[] langSysTag;  // 4-byte LangSysTag identifier
    int langSysOffset;  // Offset to LangSys table-from beginning of Script table
}
}
