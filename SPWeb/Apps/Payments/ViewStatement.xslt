<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output indent="yes" method="xml" encoding="UTF-8" />
	
	<xsl:param name="logoUrl" select="logo.jpg" />
	
	<xsl:template match="Statement">
		<div id="logoContainer">
			<img src="{$logoUrl}" alt="Header logo" />
		</div>
		<!-- addresses -->
		<div id="addressContainer">
			<div style="float:left;">
				<xsl:value-of select="concat(ServiceUser/Title, ' ', ServiceUser/FirstNames, ' ', ServiceUser/Surname)" />
				<br />
				<xsl:value-of select="ServiceAgreement/PropertyName" />
				<br />
				<xsl:value-of disable-output-escaping="yes" select="ext:Replace(ServiceAgreement/PropertyAddress, '&#xA;', '&lt;br/&gt;')" />
				<br />
				<xsl:value-of select="ServiceAgreement/PropertyPostcode" />
			</div>
			<div style="float:right;">
				<xsl:value-of select="Council/Name" />
				<br />
				<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Council/Address, '&#xA;', '&lt;br/&gt;')" />
				<br />
				<xsl:value-of select="Council/Postcode" />
				<br />
				<xsl:value-of select="Council/Phone" />
			</div>
			<div class="clearer"></div>
		</div>
		<!-- header -->
		<label class="label">Rent A/C:</label><xsl:value-of select="ServiceAgreement/AltReference" />
		<br />
		<label class="label">Council Ref:</label><xsl:value-of select="ServiceAgreement/Reference" />
		<br />
		<label class="label">Service:</label><xsl:value-of select="concat(Service/Reference, ' / ', Service/Name)" />
		<br />
		<br />
		Payments from 
		<xsl:choose>
			<xsl:when test="DateFrom"><xsl:value-of select="ext:FormatDate(DateFrom)" /></xsl:when>
			<xsl:otherwise><xsl:value-of select="ext:FormatDate(Remittance/DateFrom)" /></xsl:otherwise>
		</xsl:choose>
		 to 
		<xsl:choose>
			<xsl:when test="DateTo"><xsl:value-of select="ext:FormatDate(DateTo)" /></xsl:when>
			<xsl:otherwise>
				<xsl:variable name="maxDateTo">
					<xsl:for-each select="Remittance/RemittanceLine">
						<xsl:sort select="DateTo" data-type="text" order="descending" />
						<xsl:if test="position() = 1">
							<xsl:value-of select="DateTo" />
						</xsl:if>
					</xsl:for-each>
				</xsl:variable>
				<xsl:value-of select="ext:FormatDate($maxDateTo)" />
			</xsl:otherwise>
		</xsl:choose>		
		<br />
		<br />
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
			<xsl:variable name="initBalance" select="ext:SaveToStorage('balance', 0)" />
			<xsl:for-each select="Remittance/RemittanceLine">
				<xsl:variable name="balance" select="ext:SaveToStorage('balance', ext:GetFromStorage('balance') + Value)" />
				<tr>
					<td class="nowrap"><xsl:value-of select="ext:FormatDateCustom(DateFrom, 'dd MMM yyyy')" /></td>
					<td class="nowrap"><xsl:value-of select="ext:FormatDateCustom(DateTo, 'dd MMM yyyy')" /></td>
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
					<td class="right nowrap"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign($balance)" /></td>
				</tr>
			</xsl:for-each>
			<tr>
				<td colspan="6" class="spacer"><hr /></td>
			</tr>
			<tr>
				<td></td>
				<td></td>
				<td class="right"><strong>Totals</strong></td>
				<td class="right nowrap"><xsl:value-of select="ext:FormatCurrency(sum(Remittance/RemittanceLine[Value &gt; 0]/Value))" /></td>
				<td class="right nowrap"><xsl:value-of select="ext:FormatCurrency(sum(Remittance/RemittanceLine[Value &lt; 0]/Value))" /></td>
				<td class="right nowrap"><xsl:value-of select="ext:FormatCurrency(sum(Remittance/RemittanceLine/Value))" /></td>
			</tr>
		</tbody>			
		</table>
				
	</xsl:template>
	
</xsl:stylesheet>
