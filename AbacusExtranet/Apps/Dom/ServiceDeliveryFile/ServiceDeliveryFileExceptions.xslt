<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:ns="urn:uk.co.targetsys.abacus.imports.dom.servicedelivery.exception.v1"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">

  <xsl:output indent="yes" method="html" encoding="UTF-8" />
   <xsl:template match="ns:File">
     <div style="padding-left:1em;padding-right:1em;">
       <h1>Service Delivery File Exceptions</h1>
       <xsl:value-of select="concat('Printed at ', ext:FormatDateCustom(ext:CurrentDate(),'hh:mm:ss'), ' on ',ext:FormatDate(ext:CurrentDate()) )"/>
       <br />
       <br />
       <fieldset>
         <legend>File</legend>
         <label class="label2">Reference</label>
         <label CssClass="content">
           <xsl:value-of select="ns:Reference" />
         </label>
         <br />
         <label class="label2">Description</label>
         <label CssClass="content">
           <xsl:value-of select="ns:Description" />
         </label>
         <br />
         <label class="label2">Created</label>
         <label CssClass="content">
           <xsl:value-of select="ext:FormatDateCustom(ns:Created, 'dd/MM/yyyy')" />
         </label>
         <br />
         <xsl:if test="count(ns:Exceptions/ns:Exception) &gt; 0">
           <br />
           <xsl:call-template name="exceptions" />
         </xsl:if>
         <br />
       </fieldset>
       <xsl:for-each select="ns:ProviderContracts/ns:ProviderContract">
           <br />
           <fieldset>
             <legend>
               Provider: <xsl:value-of select="ns:ProviderRef"/> - Contract: <xsl:value-of select="ns:ContractNumber"/>
             </legend>

             <xsl:for-each select="ns:ServiceDeliveryHeaders/ns:ServiceDeliveryHeader">
               <xsl:sort select="ns:ServiceUserRef" data-type ="text" order="ascending"/>
               <xsl:sort select="ns:WeekEnding" data-type ="text" order="ascending"/>
               <div style="padding-left:0.5em;">
                 <fieldset>
                   <legend>
                     <xsl:value-of select="ns:ServiceUserRef" /> / <xsl:value-of select="ns:ServiceUserDetails" />
                   </legend>
                   <label class="label2">Weekending</label>
                   <label CssClass="content">
                     <xsl:value-of select="ext:FormatDateCustom(ns:WeekEnding, 'dd/MM/yyyy')"/>
                   </label>
                   <br />
                   <label class="label2">Reference</label>
                   <label CssClass="content">
                     <xsl:value-of select="ns:OurRef"/>
                   </label>
                   <br />
                   <label class="label2">Svc Usr Contrib.</label>
                   <label CssClass="content">
                     <xsl:value-of select="ns:ServiceUserContribution"/>
                   </label>
                   <br />
                   <label class="label2">Payment Claimed</label>
                   <label CssClass="content">
                     <xsl:value-of select="ext:FormatCurrency(ns:PaymentClaimed)"/>
                   </label>
                   <br />
                   <xsl:if test="count(ns:Exceptions/ns:Exception) &gt; 0">
                     <br />
                     <xsl:call-template name="exceptions" />
                   </xsl:if>
                   <br />
                   <xsl:for-each select="ns:HomeCareVisits/ns:HomeCareVisit">
                     <xsl:call-template name="homeCareVisit" />
                   </xsl:for-each>
                 </fieldset>
               </div>
             </xsl:for-each>
           </fieldset>
       </xsl:for-each>
     </div>
  </xsl:template>

  <!-- template to output home care Visit Details -->
  <xsl:template name="homeCareVisit">
    <div style="padding-left:0.5em;">
      <fieldset>
        <legend><xsl:value-of select="ns:ServiceType"/> / <xsl:value-of select="ns:VisitDate"/> / <xsl:value-of select="ns:ActualStartTime"/></legend>
        <label class="label2">Claimed</label>
        <label CssClass="content">
          <xsl:value-of select="ns:StartTimeClaimed"/> for <xsl:value-of select="ns:DurationClaimed"/>
        </label>
        <br />
        <label class="label2">Actual</label>
        <label CssClass="content">
          <xsl:value-of select="ns:ActualStartTime"/> for <xsl:value-of select="ns:ActualDuration"/>
        </label>
        <br />
        <label class="label2" >Secondary Visit</label>
        <label CssClass="content">
          <xsl:value-of select="ext:BoolenToYesNo(ns:SecondaryVisit)"/>
        </label>
        <br />
        <label class="label2" >No. of Carers</label>
        <label CssClass="content">
          <xsl:value-of select="ns:NumberOfCarers"/>
        </label>
        <br />
        <label class="label2" >Order Ref.</label>
        <label CssClass="content">
          <xsl:value-of select="ns:OrderRef"/>
        </label>
        <br />
        <label class="label2" >Manually Amended</label>
        <label CssClass="content">
          <xsl:value-of select="ns:ManuallyAmended"/>
        </label>
        <br />
        <label class="label2" >Visit Code</label>
        <label CssClass="content">
          <xsl:value-of select="ns:VisitCode"/>
        </label>
        <br />
        <xsl:if test="count(ns:CareWorkers/ns:CareWorker) &gt; 0">
          <br />
          <xsl:call-template name="careWorkers" />
        </xsl:if>
        <xsl:if test="count(ns:Exceptions/ns:Exception) &gt; 0">
          <br />
          <xsl:call-template name="exceptions" />
        </xsl:if>
        <br />
        
      </fieldset>
      
    </div>
  </xsl:template>

  <!-- template to output exceptions -->
  <xsl:template name="exceptions">
    <table class="listTable sortable" cellpadding="2" cellspacing="0" width="100%" summary="List of exceptions.">
      <caption>List of Exceptions.</caption>
      <thead>
        <tr>
          <th style="width:15%">Element</th>
          <th>Description</th>
        </tr>
      </thead>
      <tbody>
        <xsl:for-each select="ns:Exceptions/ns:Exception">
          <tr>
            <td><xsl:value-of select="ns:Element"/></td>
            <td style="white-space:normal;"><xsl:value-of select="ns:Description" disable-output-escaping="yes" /></td>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <!-- template to output list of Care Workers -->
  <xsl:template name="careWorkers">
    <table class="listTable sortable" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Care Workers.">
      <caption>List of Care Workers.</caption>
      <thead>
        <tr>
          <th style="width:15%">Reference</th>
          <th style="width:40%">Name</th>
        </tr>
      </thead>
      <tbody>
        <xsl:for-each select="ns:CareWorkers/ns:CareWorker">
          <tr>
            <td>
              <xsl:value-of select="ns:Reference"/>
            </td>
            <td>
              <xsl:value-of select="ns:Name"/>
            </td>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

</xsl:stylesheet>