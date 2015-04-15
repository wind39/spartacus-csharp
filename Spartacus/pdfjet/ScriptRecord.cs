using System;

namespace PDFjet.NET {
class ScriptRecord {
    byte[] scriptTag;   // 4-byte ScriptTag identifier
    int scriptOffset;   // Offset to Script table-from beginning of ScriptList
}
}
