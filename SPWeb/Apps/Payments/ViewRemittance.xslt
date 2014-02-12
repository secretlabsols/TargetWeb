<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output indent="yes" method="xml" encoding="UTF-8" />
	
	<xsl:param name="logoUrl" select="logo.jpg" />
	
	<xsl:template match="SPRemittance">
		<div id="logoContainer">
			<img src="{$logoUrl}" alt="Header logo" />
		</div>
		<!-- addresses -->
		<div id="addressContainer">
			<div style="float:left;">
				<xsl:value-of select="Council/Provider/ProviderName" />
				<br />
				<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Council/Provider/ProviderAddress, '&#xA;', '&lt;br/&gt;')" />
				<br />
				<xsl:value-of select="Council/Provider/ProviderPostcode" />
			</div>
			<div style="float:right;">
				<xsl:value-of select="Council/CouncilName" />
				<br />
				<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Council/CouncilAddress, '&#xA;', '&lt;br/&gt;')" />
				<br />
				<xsl:value-of select="Council/CouncilPostcode" />
				<br />
				<xsl:value-of select="Council/CouncilPhone" />
			</div>
			<div class="clearer"></div>
		</div>
		<!-- header -->
		<div style="float:left;width:75%;">
			<label class="label">Contract:</label><xsl:value-of select="concat(Council/Provider/Contract/ContractReference, ' / ', Council/Provider/Contract/ContractDescription)" />
			<br />
			<label class="label">Service:</label><xsl:value-of select="concat(Council/Provider/Contract/ServiceReference, ' / ', Council/Provider/Contract/ServiceName)" />
		</div>
		<div style="float:right;width:25%;">
			<label class="label">Our Ref:</label><xsl:value-of select="Council/Provider/Contract/RemittanceHeader/RemittanceNumber" />
			<br />
			<label class="label">From:</label><xsl:value-of select="Council/Provider/Contract/RemittanceHeader/DateFrom" />
			<br />
			<label class="label">To:</label><xsl:value-of select="Council/Provider/Contract/RemittanceHeader/DateTo" />
		</div>
		<div class="clearer"></div>
		<!-- details -->
		<table id="detailsTable" cellspacing="2" cellpadding="2">
		<thead>
			<tr>
				<th class="left nowrap">From</th>
				<th class="left nowrap">To</th>
				<th class="left nowrap">Detail</th>
				<th class="right nowrap">Credit (£)</th>
				<th class="right nowrap">Debit (£)</th>
				<th class="right nowrap">Balance (£)</th>
			</tr>
		</thead>
		<tbody>
			<tr>
				<td colspan="6" class="spacer"><hr /></td>
			</tr>
			<xsl:for-each select="Council/Provider/Contract/RemittanceHeader/RemittanceLine">
				<xsl:variable name="curPosition" select="position()" />
				<tr>
					<td class="nowrap"><xsl:value-of select="DateFrom" /></td>
					<td class="nowrap"><xsl:value-of select="DateTo" /></td>
					<td><xsl:value-of select="Comment" /></td>					
					<xsl:choose>
						<xsl:when test="number(Type) = 9">
							<td></td>
							<td></td>
						</xsl:when>
						<xsl:otherwise>
							<xsl:choose>
								<xsl:when test="number(Value) = 0">
									<xsl:choose>
										<xsl:when test="number(Type) &lt;= 6">
											<td class="right nowrap"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(ext:Abs(Value))" /></td>
											<td></td>
										</xsl:when>
										<xsl:otherwise>
											<td></td>
											<td class="right nowrap"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(ext:Abs(Value))" /></td>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:otherwise>
									<xsl:choose>
										<xsl:when test="number(Value) &gt; 0">
											<td class="right nowrap"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(ext:Abs(Value))" /></td>
											<td></td>
										</xsl:when>
										<xsl:otherwise>
											<td></td>
											<td class="right nowrap"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(ext:Abs(Value))" /></td>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:otherwise>
							</xsl:choose>						
						</xsl:otherwise>
					</xsl:choose>
					<td class="right nowrap"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(sum(../RemittanceLine[position() &lt;= $curPosition]/Value))" /></td>
				</tr>
			</xsl:for-each>
			<tr>
				<td colspan="6" class="spacer"><hr /></td>
			</tr>
			<tr>
				<td></td>
				<td></td>
				<td class="right"><strong>Totals</strong></td>
				<td class="right nowrap"><xsl:value-of select="ext:FormatCurrency(sum(Council/Provider/Contract/RemittanceHeader/RemittanceLine[Value &gt; 0]/Value))" /></td>
				<td class="right nowrap"><xsl:value-of select="ext:FormatCurrency(sum(Council/Provider/Contract/RemittanceHeader/RemittanceLine[Value &lt; 0]/Value))" /></td>
				<td class="right nowrap"><xsl:value-of select="ext:FormatCurrency(sum(Council/Provider/Contract/RemittanceHeader/RemittanceLine/Value))" /></td>
			</tr>
		</tbody>			
		</table>
				
	</xsl:template>
	
</xsl:stylesheet>
