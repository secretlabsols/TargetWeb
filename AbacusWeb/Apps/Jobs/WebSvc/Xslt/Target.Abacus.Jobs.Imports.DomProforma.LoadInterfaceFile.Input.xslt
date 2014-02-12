<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Domiciliary Proforma Invoice Import - Load Interface File Input.
		
		Formats the Imports.DomProforma.LoadInterfaceFile input information for display.
	-->
	<xsl:template match="ID">
		The service delivery file ID specified was: <xsl:value-of select="." /><br />
	</xsl:template>
	
</xsl:stylesheet>
