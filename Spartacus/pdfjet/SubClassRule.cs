using System;

namespace PDFjet.NET {
class SubClassRule {
    int glyphCount;
    int substCount;
    int[] classArray;                       // [glyphCount - 1]
    SubstLookupRecord[] substLookupRecord;  // [substCount]
}
}
