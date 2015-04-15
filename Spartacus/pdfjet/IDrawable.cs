/**
 * Interface that is required for components that can be drawn on a PDF page as part of Optional Content Group. 
 *
 * @author Mark Paxton
 */
namespace PDFjet.NET {
public interface IDrawable {

    /**
     * Draw the component implementing this interface on the PDF page.
     *
     * @param page the page to draw on.
     * @throws Exception
     */
    void DrawOn(Page page);
}
}   // End of namespace PDFjet.NET
