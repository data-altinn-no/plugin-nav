using GraphQL;

namespace Dan.Plugin.Nav.Services;

public interface IGraphQlRequestService
{
    GraphQLRequest CreateEmploymentHistoryRequest(string ssn);
}

public class GraphQlRequestService : IGraphQlRequestService
{
    public GraphQLRequest CreateEmploymentHistoryRequest(string ssn)
    {
        return new GraphQLRequest {
            Query ="""
                   query($finnArbeidsforholdVariabler: FinnArbeidsforholdVariabler) {
                     finnArbeidsforhold(finnArbeidsforholdVariabler: $finnArbeidsforholdVariabler) {
                       arbeidsforhold {
                         id
                         type {
                           kode
                           beskrivelse
                         }
                         arbeidssted {
                            type
                            ident
                         }
                         ansettelsesperiode {
                           startdato
                           sluttdato
                         }
                       }
                     }
                   }
                   """,
            Variables = new {
                finnArbeidsforholdVariabler = new
                {
                    arbeidstakerId = ssn,
                    arbeidsforholdtype = new[]{
                        "ordinaertArbeidsforhold",
                        "maritimtArbeidsforhold"
                    },
                    rapporteringsordning = new[]{
                        "A_ORDNINGEN"
                    },
                    arbeidsforholdstatus = new[]{
                        "AKTIV",
                        "AVSLUTTET"
                    },
                    ansettelsesdetaljerhistorikk = false
                }
            }
        };
    }
}
