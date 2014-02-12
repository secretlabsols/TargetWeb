<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Domiciliary Proforma Invoice Import - Load Interface File Input.
		
		Formats the Imports.DomProforma.LoadInterfaceFile input information for display.
	-->
	<xsl:template match="Results">
		No. service delivery headers found: <xsl:value-of select="HeadersTotal" /><br />
    No. service delivery headers processed successfully: <xsl:value-of select="HeadersProcessed" /><br />
    No. service delivery headers processed unsuccessfully: <xsl:value-of select="HeadersFailed" /><br />
    No. service delivery headers failing validation: <xsl:value-of select="HeadersInvalid" /><br />
    <br />
    <xsl:if test="OriginalFileID &gt; 0">
      <a href="FileStoreGetFile.axd?saveas=1&amp;fileDataID={OriginalFileID}">View the original service delivery file</a><br />
    </xsl:if>
    <xsl:if test="ExceptionsFileID &gt; 0">
      <a href="FileStoreGetFile.axd?saveas=1&amp;fileDataID={ExceptionsFileID}">View the service delivery exceptions file</a><br />
    </xsl:if>
  </xsl:template>
	
</xsl:stylesheet>
