<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Direct Payment Contracts Import - LoadInterfaceFile Input.
		
		Formats the Target.Abacus.Jobs.Imports.DirectPaymentContracts.LoadInterfaceFile input information for display.
	-->

    <xsl:template match="/">

        <xsl:apply-templates select="//FilterFilePath" />

    </xsl:template>
	
	<xsl:template match="FilterFilePath">
        The interface file specified was: <xsl:value-of select="." /><br />
	</xsl:template>
	
</xsl:stylesheet>
