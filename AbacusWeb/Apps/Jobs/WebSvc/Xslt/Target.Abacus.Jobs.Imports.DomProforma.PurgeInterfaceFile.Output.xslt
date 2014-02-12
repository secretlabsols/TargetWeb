<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Domiciliary Proforma Invoice Import - Purge Spent Service Delivery Data.
		
		Formats the Imports.DomProforma.PurgeInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
		No. spent invoice batches found: <xsl:value-of select="BatchesFound" /><br />
    No. spent invoice batches deleted: <xsl:value-of select="BatchesDeleted" /><br />
    No. unconfirmed service delivery files deleted: <xsl:value-of select="InterfaceFilesDeleted" /><br />
  </xsl:template>
	
</xsl:stylesheet>
