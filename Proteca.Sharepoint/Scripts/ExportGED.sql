-- Notes on usage:
-- This script presumes the existence of the Oracle directory "GED_EXPORT".
-- After execution, the document files, along with manifest.xml file will be output to the physical path pointed to by "GED_EXPORT".
-- The total number of files exported can be examined with the "index" attibute on the Document node.

DECLARE
	l_manifest_file UTL_FILE.FILE_TYPE;
	l_doc_file UTL_FILE.FILE_TYPE;
  l_buffer RAW(32767);	
	MAXBUFSIZE INTEGER := 32767;
	l_pos INTEGER;
	l_remainder INTEGER; 
	l_blob BLOB;
	l_blob_len INTEGER;
	l_file_name VARCHAR2(100);
  l_src_file_name VARCHAR2(120);
  l_index INTEGER := 0;
  
BEGIN

	l_manifest_file := UTL_FILE.fopen_nchar('GED_EXPORT', 'manifest.xml', 'w'); -- File to be created with unicode encoding
	UTL_FILE.put_line_nchar(l_manifest_file, '<?xml version="1.0" encoding="UTF-8"?>'); 
  UTL_FILE.put_line_nchar(l_manifest_file, '<documents>');
  
  UPDATE NOMENCLATURE_TYPE_DOC
  SET DESIGNATION = replace(DESIGNATION, '...', 'etc');
  
	FOR doc_rec IN (
		SELECT GED.*, TO_CHAR(GED.DATE_ENREGISTREMENT,'dd/mm/yyyy HH24:MI:SS') AS CREATED,
			NOMENCLATURE_TYPE_DOC.TYPE_OUVRAGE,
			NOMENCLATURE_TYPE_DOC.TYPE_DOSSIER,
      NOMENCLATURE_TYPE_DOC.DESIGNATION
		FROM GED
		LEFT OUTER JOIN NOMENCLATURE_TYPE_DOC
		ON GED.CLE_NOMENCLATURE = NOMENCLATURE_TYPE_DOC.CLE_NOMENCLATURE AND NOMENCLATURE_TYPE_DOC.TYPE_PROFIL_NIVEAU1 = 1
		--WHERE ROWNUM <= 100
	)
	LOOP
		-- Manifest
    l_index := l_index + 1;
		l_blob_len := DBMS_LOB.getlength(doc_rec.DOCUMENT);
    
    l_file_name := trim(replace(replace(replace(doc_rec.LIBELLE, chr(38),  '_et_'), '[', '('), ']', ')')); -- character '&', '[' and ']' is not valid as file name for SharePoint.
		l_src_file_name := doc_rec.CLE_DOCUMENT || '_' || l_file_name;
     
		UTL_FILE.put_line_nchar(l_manifest_file, '  <document id="' || doc_rec.CLE_DOCUMENT || '" index="' || l_index ||'">');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <fileName>' || l_file_name || '</fileName>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <sourceFileName>' || l_src_file_name || '</sourceFileName>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <date>' || trim(doc_rec.CREATED) || '</date>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <version>' || trim(doc_rec.NO_ENREGISTREMENT) || '</version>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <isArchived>' || trim(doc_rec.ARCHIVE) || '</isArchived>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <typeEquipment>' || trim(doc_rec.TYPE_EQUIPEMENT) || '</typeEquipment>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <cleOuvrage>' || trim(doc_rec.CLE_OUVRAGE) || '</cleOuvrage>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <typeOuvrage>' || trim(doc_rec.TYPE_OUVRAGE) || '</typeOuvrage>');
		UTL_FILE.put_line_nchar(l_manifest_file, '    <typeDossier>' || trim(doc_rec.TYPE_DOSSIER) || '</typeDossier>');
    UTL_FILE.put_line_nchar(l_manifest_file, '    <designation>' || trim(doc_rec.DESIGNATION) || '</designation>');
		UTL_FILE.put_line_nchar(l_manifest_file, '  </document>');
    
		-- Document file
		l_doc_file := UTL_FILE.fopen('GED_EXPORT', l_src_file_name, 'wb', MAXBUFSIZE);
    
		l_pos := 1;
    
  	WHILE l_pos < l_blob_len LOOP
      /*
      -- The code below seems to work. But it soons starts to slow down after the 6th record...
      -- http://stackoverflow.com/questions/4392491/oracle-blob-extraction-very-slow
      -- https://forums.oracle.com/forums/thread.jspa?threadID=1118281      
    	
      DBMS_LOB.read(doc_rec.DOCUMENT, MAXBUFSIZE, l_pos, l_buffer);
      
      */
			l_remainder := l_blob_len - l_pos + 1;
			IF (l_remainder >= MAXBUFSIZE) THEN 
			DBMS_LOB.read(doc_rec.DOCUMENT, MAXBUFSIZE, l_pos, l_buffer);
			ELSE
			DBMS_LOB.read(doc_rec.DOCUMENT, l_remainder, l_pos, l_buffer);
			END IF;
     
			UTL_FILE.put_raw(l_doc_file, l_buffer, TRUE);
			l_pos := l_pos + MAXBUFSIZE;
      
		END LOOP;
		UTL_FILE.fclose(l_doc_file);
    
	END LOOP;

	UTL_FILE.put_line_nchar(l_manifest_file, '</documents>');
	UTL_FILE.fclose(l_manifest_file);
  
	EXCEPTION
	WHEN OTHERS THEN
	BEGIN
		IF UTL_FILE.is_open(l_manifest_file) THEN
		UTL_FILE.fclose(l_manifest_file);
		END IF;
		IF UTL_FILE.is_open(l_doc_file) THEN
		UTL_FILE.fclose(l_doc_file);
		END IF;
		RAISE;
	END;
END;