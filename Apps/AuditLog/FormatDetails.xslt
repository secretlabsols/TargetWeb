<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output indent="yes" method="html" encoding="UTF-8" />

	<xsl:template match="logData">
		<h1>Audit Log</h1>
		<xsl:apply-templates select="event" />
	</xsl:template>
	
	<xsl:template match="event">

		<table cellpadding="0" cellspacing="0" width="97%" border="0">
			<tr>
				<td>
					<strong>Date</strong>
				</td>
				<td>
					<xsl:value-of select="ext:FormatDateCustom(@date, 'dd/MM/yyyy HH:mm:ss')"/>
				</td>
				<td>
					<strong>User</strong>
				</td>
				<td>
					<xsl:value-of select="@userName"/>
				</td>
			</tr>
			<tr>
				<td>
					<strong>Area</strong>
				</td>
				<td>
					<xsl:value-of select="@tableName"/>
				</td>
				<td>
					<strong>Screen</strong>
				</td>
				<td>
					<xsl:value-of select="title"/>
				</td>
			</tr>
			<tr>
				<td>
					<strong>Type</strong>
				</td>
				<td>
					<xsl:value-of select="@typeDesc"/>
				</td>
				<td></td>
				<td></td>
			</tr>
		</table>
		<br />
		<table cellpadding="0" cellspacing="0" width="97%" border="0">
			<tr>
				<th style="text-align:left;">Description</th>
				<th style="text-align:left;">Old Value</th>
				<th style="text-align:left;">New Value</th>
			</tr>
			<xsl:apply-templates select="items/item[@always = 'True']">
				<xsl:sort select="@name"/>
			</xsl:apply-templates>
			<xsl:apply-templates select="items/item[@always = 'False']">
				<xsl:sort select="@name"/>
			</xsl:apply-templates>
		</table>
		<hr />
		
	</xsl:template>

	<xsl:template match="item">

		<tr>
			<td>
				<xsl:value-of select="ext:SplitOnCapitals(@name)" />
			</td>
			<td>
				<xsl:value-of select="old"/>
			</td>
			<td>
				<xsl:value-of select="new"/>
			</td>
		</tr>
		
	</xsl:template>
	
</xsl:stylesheet>
