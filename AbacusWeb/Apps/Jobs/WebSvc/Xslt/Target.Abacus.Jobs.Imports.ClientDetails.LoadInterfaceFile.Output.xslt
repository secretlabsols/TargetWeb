<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Client Details Import - Load Interface File Output.
		
		Formats the Imports.ClientDetails.LoadInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
		<xsl:value-of select="Count" /> clients were processed from
		<xsl:choose>
			<xsl:when test="string-length(InterfaceFile) = 0">(file not specified)</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="InterfaceFile" />.
				<br />
				<xsl:if test="string-length(InterfaceFileURL) &gt; 0">
					<a href="{InterfaceFileURL}">View a copy of this file.</a>
				</xsl:if>
                <xsl:if test="number(ExceptionsCount) &gt; 0">
                    <br /><br />
                    <b>IMPORTANT</b>
                    <br />
                    <xsl:value-of select="ExceptionsCount" /> exception(s) were raised during processing. These clients were not processed.
                    <br />
                    Please see the exceptions report for more details:
                    <br />
                    View the report <a href="{ExceptionsCsvFileUrl}">in Excel</a>
                    <br />
                    View the report <a href="{ExceptionsXmlFileUrl}">as XML</a>
                </xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
</xsl:stylesheet>
