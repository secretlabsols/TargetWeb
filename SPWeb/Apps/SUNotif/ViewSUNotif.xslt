<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ext="urn:ext-util">
	<xsl:output indent="yes" method="html" encoding="UTF-8" />

	<xsl:param name="processed" select="0" />

	<xsl:template match="ViewableSUNotif">
	
		<xsl:variable name="councilName" select="CouncilName" />
		<xsl:variable name="councilAdminAuthority" select="CouncilAdminAuthority" />
		<xsl:variable name="councilAddress" select="CouncilAddress" />
		<xsl:variable name="councilPostcode" select="CouncilPostcode" />
		<xsl:variable name="councilFax" select="CouncilFax" />	
	
		<h1>Application for Supporting People Grant</h1>
		
		<hr />
	
		<xsl:if test="$processed = 1">
			<fieldset>
				<legend>Outcome</legend>
				This notification has been processed and its outcome is displayed below.
				<br /><br />
				<strong>Decision: </strong><xsl:value-of select="StatusDesc" />
				<br /><br />
				<strong>Processed By: </strong><xsl:value-of select="ProcessedBy" />
				<br /><br />
				<strong>Processed On: </strong><xsl:value-of select="ext:FormatDate(CompletedDate)" />
				<br /><br />
				<strong>Comment: </strong><xsl:value-of select="Comment" />
				<br />
			</fieldset>
		</xsl:if>
	
		<h3>1. Your name and personal details</h3>
		<div class="section">
			<strong>Name: </strong><xsl:value-of select="concat(PrimaryTitle, ' ', PrimaryFirstNames, ' ', PrimaryLastName)" />
			<br /><br />
			<strong>National Insurance No: </strong><xsl:value-of select="PrimaryNINo" />
			<br /><br />
			<strong>Date of birth: </strong><xsl:value-of select="ext:FormatDate(PrimaryBirthDate)" />
		</div>
		
		<h3>2. Your partner and their personal details</h3>
		<div class="section">
			<xsl:variable name="secondaryName">
				<xsl:choose>
					<xsl:when test="string-length(SecondaryLastName) &gt; 0"><xsl:value-of select="concat(SecondaryTitle, ' ', SecondaryFirstNames, ' ', SecondaryLastName)" /></xsl:when>
					<xsl:otherwise>[Not Entered]</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<xsl:variable name="secondaryNINO">
				<xsl:choose>
					<xsl:when test="string-length(SecondaryLastName) &gt; 0"><xsl:value-of select="SecondaryNINo" /></xsl:when>
					<xsl:otherwise>[Not Entered]</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<xsl:variable name="secondaryBirthDate">
				<xsl:choose>
					<xsl:when test="string-length(SecondaryLastName) &gt; 0"><xsl:value-of select="ext:FormatDate(SecondaryBirthDate)" /></xsl:when>
					<xsl:otherwise>[Not Entered]</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<strong>Name: </strong><xsl:value-of select="$secondaryName" />
			<br /><br />
			<strong>National Insurance No: </strong><xsl:value-of select="$secondaryNINO" />
			<br /><br />
			<strong>Date of birth: </strong><xsl:value-of select="$secondaryBirthDate" />
		</div>
				
		<h3>3. Your current address</h3>
		<div class="section">
			<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Address, '&#xA;', '&lt;br/&gt;')" />
			<br />
			<xsl:value-of select="Postcode" />
		</div>
		
		<h3>4. Your supported housing or sheltered housing service</h3>
		<div class="section">
			<strong>Name of service provider: </strong><xsl:value-of select="ProviderName" />
			<br /><br />
			<strong>Name of support service: </strong><xsl:value-of select="ServiceName" />
			<br /><br />
			<strong>Date you will start/expect to start receiving the support service: </strong><xsl:value-of select="ext:FormatDate(ServiceStartDate)" />
		</div>
		
		<h3>5. Your tenancy or support agreement</h3>
		<div class="section">
			I <strong><xsl:choose><xsl:when test="TenancyServiceAgreement = 'True'">have</xsl:when><xsl:otherwise>have not</xsl:otherwise></xsl:choose></strong> already signed a tenancy agreement or support agreement for the service described in section 4.
		</div>
		
		<h3>6. What will you do with my personal information?</h3>
		<div class="section">
			<p>
				The supporting People Team will have to exchange some information with other organisations. You will need to consent 
				to this exchange of information so that Supporting People Grant can be paid, if you are eligible. By giving us 
				this consent, you agree that:
			</p>
			<ul>
				<li>
					Your local Housing Benefit office can inform the <xsl:value-of select="$councilAdminAuthority" /> Supporting 
					People Team (based in <xsl:value-of select="$councilName" />) whether you have made a Housing Benefit 
					application, and the progress of that application.
				</li>
				<li>
					Your local housing Benefit office can inform the Supporting People Team whether or not you are assessed as 
					eligible for Housing Benefit.
				</li>
				<li>
					The Supporting People Team and your local Housing Benefit office can exchange such information as is necessary 
					to investigate and prevent fraud.
				</li>
				<li>
					The Supporting People Team can inform the provider of your supported housing or sheltered housing service that 
					you have applied for Supporting People Grant, and the progress of that application.
				</li>
				<li>
					The Supporting People Team can inform the provider of your supported housing or sheltered housing service how 
					much of the support charge will be paid by Supporting People Grant, if any; and of any adjustments that may 
					need to be made to that Grant if you fail to notify us of any change of circumstances.
				</li>
				<li>
					The provider of your supported housing or sheltered housing service can inform the Supporting People Team if 
					they become aware that you cease to receive the support service or that you have had a significant change of 
					financial circumstances.
				</li>
			</ul>
		</div>
		
		<h3>7. Declarations</h3>
		<div class="section">
			I declare that, to the best of my knowledge, the information I have given on this form is correct. I understand that, 
			should I be eligible for Supporting People Grant, I must inform the Supporting People Team if I stop receiving this 
			service or if I have a change of financial circumstances.
			<br /><br /><br />
			<strong>Signed: </strong>...........................................................................................
			<br />
			<br />
			<strong>Date: </strong>......................................
			<br /><br /><br />
			I give my consent for information to be exchanged between <xsl:value-of select="$councilAdminAuthority" /> Supporting 
			People Team and Housing Benefit sections, and between the <xsl:value-of select="$councilAdminAuthority" /> Supporting 
			People Team and the Supporting People service provider named in section 4 above, as specified in section 6 above.
			<br /><br /><br />
			<strong>Signed: </strong>...........................................................................................
			<br /><br />
			<strong>Date:   </strong>......................................
		</div>		
		<br /><br />
		<hr />
		<br />
		
		<div class="section">
			<strong>Please scan this signed form and upload the scanned file, or</strong>
			<br /><br />
			<strong>send to:</strong>
			<br />
			<xsl:value-of select="$councilAdminAuthority" /> Supporting People Team
			<br />
			<xsl:value-of disable-output-escaping="yes" select="ext:Replace($councilAddress, '&#xA;', '&lt;br/&gt;')" />
			<br />
			<xsl:value-of select="$councilPostcode" />
			<br /><br />
			<strong>or fax to:</strong>
			<br />
			<xsl:value-of select="$councilFax" />
			<br /><br />
		</div>
	
	</xsl:template>

</xsl:stylesheet>
