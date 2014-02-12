<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	
	<!--
		Abacus for Windows Job Service - Dom Svc Orders Import - Remove Existing Data V2 Output.
		
		Formats the Imports.DomSvcOrders.RemoveExistingDataV2 output information for display.
	-->
		
	<xsl:template match="Results">
		<xsl:value-of select="ClientContractCount" /> client/contract pairing(s) processed.
		<br />
		<xsl:value-of select="TotalDSOsFound" /> existing domiciliary service order record(s) found for the client/contract pairing(s).
		<br />
		<xsl:value-of select="TotalDSOsDeleted" /> domiciliary service order record(s) deleted.
		<br /><br />
		<xsl:if test="number(TotalDSOsNotDeleted) &gt; 0">
			<b>IMPORTANT</b>
			<br />
			<xsl:value-of select="TotalDSOsNotDeleted" /> service order(s) were not deleted during processing.
			<br />
        </xsl:if>
        <xsl:if test="ActionsCsvFileUrl != ''">
            Please see the actions report for more details:
            <br />
		    View the report <a href="{ActionsCsvFileUrl}">in Excel</a>
    		<br />
	    	View the report <a href="{ActionsXmlFileUrl}">as XML</a>
        </xsl:if>
	</xsl:template>
	
</xsl:stylesheet>
