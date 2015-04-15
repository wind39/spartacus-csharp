using System;

namespace PDFjet.NET {
class SubRule {
    int glyphCount;
    int substCount;
    int[] input;                            // [glyphCount - 1]
    SubstLookupRecord[] substLookupRecord;  // [substCount]
}
}
