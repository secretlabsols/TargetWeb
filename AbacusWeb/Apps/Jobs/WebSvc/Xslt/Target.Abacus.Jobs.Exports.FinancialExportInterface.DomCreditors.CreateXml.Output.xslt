<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Client Details Import - Load Interface File Output.
		
		Formats the Imports.ClientDetails.LoadInterfaceFile output information for display.
	-->
	<xsl:template match="Results">
        <xsl:choose>
			<xsl:when test="string-length(OriginalXML_URL) = 0">(original XML file not specified)</xsl:when>
			<xsl:otherwise>
				<xsl:if test="string-length(OriginalXML_URL) &gt; 0">
					<a href="{OriginalXML_URL}"> View a copy of the original XML file before processing.</a>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
        <br />
        <xsl:choose>
            <xsl:when test="string-length(WorkingXML_URL) = 0">(modified XML file not specified)</xsl:when>
            <xsl:otherwise>
                <xsl:if test="string-length(WorkingXML_URL) &gt; 0">
                    <a href="{WorkingXML_URL}"> View a copy of the modified XML file after processing.</a>
                </xsl:if>
            </xsl:otherwise>
        </xsl:choose>

        <xsl:if test="number(ExceptionsFound) &gt; 0">
            <xsl:if test="string-length(ExceptionsHTML_URL) &gt; 0">
                <br />
                <br />
                <b>
                    <xsl:value-of select="ExceptionsFound" /> exception(s) found during processing of the data.
                </b>
                <a href="{ExceptionsHTML_URL}" target="_blank"> View the exception(s) found during processing.</a>
            </xsl:if>
        </xsl:if>
    </xsl:template>
	
</xsl:stylesheet>
