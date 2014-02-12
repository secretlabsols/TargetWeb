<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output indent="yes" method="html" encoding="UTF-8" />
	
	<xsl:template match="StatementData">
		
		<table id="detailsTable" cellpadding="0" cellspacing="0">
		<!-- repeated header section -->
		<thead>
			<tr>
				<td colspan="7">
					<!-- logo -->
					<div id="logoContainer">
						<img src="GetSystemLogo.axd" alt="Header logo" />
					</div>
					<!-- council address -->
					<div id="councilAddressContainer">
						<div id="councilName"><xsl:value-of select="Header/Item/SiteName" /></div>
						<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Header/Item/SiteAddress, '&#xA;', '&lt;br/&gt;')" />
					</div>
					<div class="clearer"></div>
				</td>
			</tr>
			<tr>
				<td colspan="7">
					<!-- service user name and home address -->
					<div id="homeAddressContainer">
						<div id="homeName"><xsl:value-of select="Data/Line/HomeName" /></div>
						<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Data/Line/HomeAddress, '&#xA;', '&lt;br/&gt;')" />
						<br />
					</div>
				</td>
			</tr>
			<!-- statement title -->
			<tr>
				<td class="statementTitle" colspan="7">
					<xsl:value-of disable-output-escaping="yes" select="concat('Statement of Payments for &quot;', Data/Line/ClientName, '&quot; (', Data/Line/ClientRef, ')')" />
				</td>
			</tr>
			<tr>
				<td class="statementSubTitle" colspan="7">
					Payments re: Residential/Nursing Placement Relating to the Period from
					<xsl:choose>
						<xsl:when test="string-length(Header/Item/DateFrom) &gt; 0"><xsl:value-of select="ext:FormatDateCustom(Header/Item/DateFrom, 'dd/MM/yyyy')" /></xsl:when>
						<xsl:otherwise>
							<xsl:variable name="minDateFrom">
								<xsl:for-each select="Data/Line">
									<xsl:sort select="LineDateFrom" data-type="text" order="ascending" />
									<xsl:if test="position() = 1">
										<xsl:value-of select="LineDateFrom" />
									</xsl:if>
								</xsl:for-each>
							</xsl:variable>
							<xsl:value-of select="ext:FormatDateCustom($minDateFrom, 'dd/MM/yyyy')" />
						</xsl:otherwise>
					</xsl:choose>
					to 
					<xsl:choose>
						<xsl:when test="string-length(Header/Item/DateTo) &gt; 0"><xsl:value-of select="ext:FormatDateCustom(Header/Item/DateTo, 'dd/MM/yyyy')" /></xsl:when>
						<xsl:otherwise>
							<xsl:variable name="maxDateTo">
								<xsl:for-each select="Data/Line">
									<xsl:sort select="LineDateTo" data-type="text" order="descending" />
									<xsl:if test="position() = 1">
										<xsl:value-of select="LineDateTo" />
									</xsl:if>
								</xsl:for-each>
							</xsl:variable>
							<xsl:value-of select="ext:FormatDateCustom($maxDateTo, 'dd/MM/yyyy')" />
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</tr>
			<!-- detail lines header -->
			<tr class="detailHeader">
				<td class="leftMostColumn">Reference</td>
				<td class="dateColumn">From</td>
				<td class="dateColumn">To</td>
				<td class="detailColumn">Detail</td>
				<td class="dataColumn">Credit</td>
				<td class="dataColumn">Debit</td>
				<td class="dataColumn borderRightThick">Balance</td>
			</tr>
		</thead>
		<!-- end of repeated header section -->
		<tbody>
			<xsl:variable name="initBalance" select="ext:SaveToStorage('balance', 0)" />
			<xsl:for-each select="Data/Line">
				<!-- output a line -->
				<xsl:call-template name="remittanceLine" />
			</xsl:for-each>
			<!-- statement sub-totals -->
			<tr>
				<td class="statementSubTotalTitle" colspan="4">Sub-totals</td>
				<td class="statementSubTotalValue">
					<xsl:value-of select="ext:FormatCurrency(sum(Data/Line[LineValue &gt; 0]/LineValue))" />
				</td>
				<td class="statementSubTotalValue">
					<xsl:value-of select="ext:FormatCurrency(sum(Data/Line[LineValue &lt; 0]/LineValue))" />
				</td>
				<td class="statementSubTotalValue borderRightThick">
					<xsl:value-of select="ext:FormatCurrency(ext:GetFromStorage('balance'))" />
				</td>
			</tr>
			<!-- statement total -->
			<tr>
				<td class="statementTotalTitle" colspan="4">Total</td>
				<td class="statementTotalValue" colspan="2">&#160;</td>
				<td class="statementTotalValue borderRightThick">
					<xsl:value-of select="ext:FormatCurrency(ext:GetFromStorage('balance'))" />
				</td>
			</tr>
		</tbody>
		<tfoot>
			<!-- repeated footer line -->
			<tr>
				<td class="footer" colspan="7">&#160;</td>
			</tr>
			<!-- end of repeated footer line -->
		</tfoot>
		</table>
		<br />
	
	</xsl:template>
		
	<!-- template to output a line -->
	<xsl:template name="remittanceLine">	
		
		<xsl:variable name="balance" select="ext:SaveToStorage('balance', ext:GetFromStorage('balance') + LineValue)" />
		<tr>
			<td class="leftMostColumn">
				<xsl:value-of select="RemittanceNumber" />
			</td>
			<td class="dateColumn">
				<xsl:value-of select="ext:FormatDateCustom(LineDateFrom, 'dd/MM/yyyy')" />
			</td>
			<td class="dateColumn">
				<xsl:value-of select="ext:FormatDateCustom(LineDateTo, 'dd/MM/yyyy')" />
			</td>
			<td class="detailColumn">
				<xsl:choose>
					<xsl:when test="string-length(TextLine) = 0">
						Standard Payment
						<br />
						<xsl:variable name="duration" select="ext:PrintWeeksDays(Days, '{0} Weeks {1} Days')" />
						<xsl:value-of select="concat('(', $duration, ' @ ', ext:FormatCurrency(NetRate), ' per week)')" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of disable-output-escaping="yes" select="ext:Replace(TextLine, '&#xA;', '&lt;br/&gt;')" />
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="dataColumn">
				<xsl:choose>
					<xsl:when test="LineValue &gt;= 0"><xsl:value-of select="ext:FormatCurrency(LineValue)" /></xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="dataColumn">
				<xsl:choose>
					<xsl:when test="LineValue &lt; 0"><xsl:value-of select="ext:FormatCurrency(LineValue)" /></xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="dataColumn borderRightThick">
				<xsl:value-of select="ext:FormatCurrency($balance)" />
			</td>
		</tr>			
			
	</xsl:template>
		
</xsl:stylesheet>
