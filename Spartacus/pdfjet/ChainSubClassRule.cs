using System;

namespace PDFjet.NET {
class ChainSubClassRule {
    int backtrackGlyphCount;
    int[] backtrack;                // [backtrackGlyphCount]
    int inputGlyphCount;
    int[] input;                    // [inputGlyphCount - 1]
    int lookaheadGlyphCount;
    int[] lookahead;                // [lookaheadGlyphCount]
    int substCount;
    SubstLookupRecord[] substLookupRecord;  // [substCount]
}
}
