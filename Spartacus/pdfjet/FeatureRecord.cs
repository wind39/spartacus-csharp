using System;

namespace PDFjet.NET {
class FeatureRecord {
    byte[] featureTag;  // 4-byte feature identification tag
    int featureOffset;  // Offset to Feature table-from beginning of FeatureList
}
}
