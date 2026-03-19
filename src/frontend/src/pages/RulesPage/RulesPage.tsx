import { useEffect, useMemo, useState } from 'react';
import PageTemplate from '../../components/PageTemplate/PageTemplate';
import LoadingSpinner from '../../components/LoadingSpinner/LoadingSpinner';
import DOMPurify from 'dompurify';
import { getPageContent, type PageContentResponse } from '../../services/pageContentService';
import './RulesPage.scss';

function RulesPage() {
  const [content, setContent] = useState<PageContentResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      try {
        setIsLoading(true);
        setError(null);

        const data = await getPageContent('saannot');
        if (!cancelled) {
          setContent(data);
        }
      } catch (e) {
        const message = e instanceof Error ? e.message : 'Failed to load rules.';
        if (!cancelled) {
          setError(message);
          setContent(null);
        }
      } finally {
        if (!cancelled) {
          setIsLoading(false);
        }
      }
    };

    load();

    return () => {
      cancelled = true;
    };
  }, []);

  const sanitizedHtml = useMemo(() => {
    if (!content?.contentHtml) return '';
    return DOMPurify.sanitize(content.contentHtml);
  }, [content?.contentHtml]);

  return (
    <PageTemplate title={content?.title?.trim() || 'Säännöt'}>
      <div className="rules-container">
        {isLoading ? (
          <div className="flex justify-center items-center min-h-screen">
            <LoadingSpinner text="Ladataan sääntöjä..." />
          </div>
        ) : error ? (
          <div className="flex flex-col gap-2">
            <p>Sääntöjen lataaminen epäonnistui.</p>
            <p className="text-sm text-gray-600">{error}</p>
          </div>
        ) : !sanitizedHtml ? (
          <p>Sääntöjä ei ole vielä tallennettu.</p>
        ) : (
          <div
            className="rules-html"
            // HTML is sanitized via DOMPurify before rendering.
            dangerouslySetInnerHTML={{ __html: sanitizedHtml }}
          />
        )}
      </div>
    </PageTemplate>
  );
}

export default RulesPage; 